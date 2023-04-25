using System.Diagnostics;
using A2.Shared;
using Confluent.Kafka;
using Newtonsoft.Json;

var consumerConfig = new ConsumerConfig()
{
    BootstrapServers = "localhost:9092",
    GroupId = "Group1"
};

var foundNums = new List<int>();

Console.WriteLine("Connecting to Kafka ...");
using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

consumer.Subscribe(Constants.Kafka.TopicName);

(List<int>, long) GetPrimes(int b)
{
    Console.WriteLine($"Calculating Primes For {b}");
    var nums = new List<int>();
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    var startIdx = 1;

    if (foundNums.Any())
    {
        nums = foundNums.Where(n => n <= b).ToList();
        if (foundNums.Last() >= b)
        {
            stopwatch.Stop();
            return (nums, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            startIdx = foundNums.Last() + 1;
        }
    }

    for (var i = startIdx; i <= b; i++)
    {
        if (i is 1 or 0) continue;

        var flag = 1;

        for (var j = 2; j <= i / 2; ++j)
        {
            if (i % j != 0) continue;
            flag = 0;
            break;
        }

        if (flag == 1) nums.Add(i);
    }
    
    foundNums.AddRange(nums);
    foundNums.Sort();
    
    stopwatch.Stop();

    return (nums, stopwatch.ElapsedMilliseconds);
}

Console.WriteLine("Waiting for first message ...");
while (true)
{
    var consumeResult = consumer.Consume();
    var message = JsonConvert.DeserializeObject<QuestionMessage>(consumeResult.Message.Value);
    Console.WriteLine(consumeResult.Message.Value);
    var (primes, ms) = GetPrimes(Convert.ToInt32(message.Question));
    
    Console.WriteLine($"calculated primes  [<List>[{primes.Count}]] in {ms}ms");
}
consumer.Close();