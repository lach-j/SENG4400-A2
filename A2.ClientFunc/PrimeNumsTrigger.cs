using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
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
        Console.WriteLine($"Calculating Primes For {b}");

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var primeSet = new HashSet<int>();
        var primes = new List<int>();

        for (var x = 2; x < b; x++)
        {
            for (var y = x * 2; y < b; y += x)
            {
                primeSet.Add(y);
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

        Console.WriteLine($"calculated primes  [<List>[{primes.Count}]] in {ms}ms");

        try
        {
            var client = new HttpClient();

            var username = GetEnvironmentVariable("A2Dashboard_USERNAME");
            var password = GetEnvironmentVariable("A2Dashboard_PASSWORD");
            
            var authenticationString = $"{username}:{password}";
            var base64EncodedAuthenticationString =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            
            var baseUrl = GetEnvironmentVariable("A2Dashboard_BASEURL");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/Answers");
            requestMessage.Headers.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");
            requestMessage.Content =
                new StringContent(JsonConvert.SerializeObject(new AnswerDto { TimeTaken = ms, Answer = primes }),
                    Encoding.UTF8, "application/json");
            var response = await client.SendAsync(requestMessage);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }
}