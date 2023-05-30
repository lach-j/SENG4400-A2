using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using A2.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace A2.ClientFunc;

public static class PrimeNumsTrigger
{
    private static (List<int>, long) GetPrimes(int b)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var primeSet = new HashSet<int>();
        var primes = new List<int>();

        for (var i = 2; i < b; i++)
        {
            for (var j = i * 2; j < b; j += i)
            {
                primeSet.Add(j);
            }
        }

        for (var z = 2; z < b; z++)
        {
            if (!primeSet.Contains(z))
            {
                primes.Add(z);
            }
        }

        stopwatch.Stop();
        return (primes, stopwatch.ElapsedMilliseconds);
    }
    
    private static string GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }

    [FunctionName("PrimeNumsTrigger")]
    public static async Task RunAsync(
        [ServiceBusTrigger(Constants.ServiceBus.QueueName, Connection = "A2ServiceBus_SERVICEBUS")]
        QuestionMessage message, ILogger log)
    {
        var (primes, ms) = GetPrimes(Convert.ToInt32(message.Question));

        try
        {
            var client = new HttpClient();

            var username = GetEnvironmentVariable("A2Dashboard_USERNAME");
            var password = GetEnvironmentVariable("A2Dashboard_PASSWORD");
            
            var authenticationString = $"{username}:{password}";
            var base64EncodedAuthenticationString =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            var answerDto = new AnswerDto { TimeTaken = ms, Answer = primes };
            
            var pairs = answerDto.GetType()
                .GetProperties()
                .Select(p => new {
                    Property = p,
                    Attribute = p
                        .GetCustomAttributes(
                            typeof(JsonPropertyAttribute), true)
                        .Cast<JsonPropertyNameAttribute>()
                        .FirstOrDefault() });
            
            var answer = JsonConvert.SerializeObject(answerDto);
            
            var baseUrl = GetEnvironmentVariable("A2Dashboard_BASEURL");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/Answers");
            requestMessage.Headers.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");
            requestMessage.Content =
                new StringContent(answer,
                    Encoding.UTF8, "application/json");
            log.LogInformation(answer);
            await client.SendAsync(requestMessage);
        }
        catch (Exception e)
        {
            log.LogError(e.Message, e);
        }
    }
}