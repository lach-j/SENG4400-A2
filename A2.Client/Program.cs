using System.Diagnostics;
using System.Text;
using A2.Shared;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        var completionSource = new TaskCompletionSource<object>();
        var cancellationToken = new CancellationToken();

        var sbClient = new ServiceBusClient("Endpoint=sb://a2-servicebus.servicebus.windows.net/;SharedAccessKeyName=ClientAccess;SharedAccessKey=hH6mOSprWSK3ZsaQ0LmUCb1eF/gFKypiN+ASbFAy5ws=;EntityPath=primenums", clientOptions);
        var listener = sbClient.CreateProcessor("primenums");

        listener.ProcessMessageAsync += MessageHandler;
        listener.ProcessErrorAsync += ErrorHandler;

        await listener.StartProcessingAsync(cancellationToken);

        await completionSource.Task;

        (List<int>, long) GetPrimes(int b)
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


        async Task MessageHandler(ProcessMessageEventArgs args)
        {

            var consumeResult = args.Message.Body.ToString();
            var message = JsonConvert.DeserializeObject<QuestionMessage>(consumeResult);
            var (primes, ms) = GetPrimes(Convert.ToInt32(message.Question));

            Console.WriteLine($"calculated primes  [<List>[{primes.Count}]] in {ms}ms");

            try
            {
                var client = new HttpClient();
                const string authenticationString = "admin:password";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7189/api/Answers");
                requestMessage.Headers.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");
                requestMessage.Content =
                    new StringContent(JsonConvert.SerializeObject(new AnswerDto() { TimeTaken = ms, Answer = primes }),
                        Encoding.UTF8, "application/json");
                var response = await client.SendAsync(requestMessage);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            await args.CompleteMessageAsync(args.Message);
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}