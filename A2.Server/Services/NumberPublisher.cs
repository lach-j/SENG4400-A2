using A2.Server.Configuration;
using A2.Shared;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace A2.Server.Services;

public class NumberPublisher
{
    private readonly AppSettings _appSettings;
    private readonly KafkaPubSub _kafka;
    private readonly ILogger<NumberPublisher> _logger;

    public NumberPublisher(IOptions<AppSettings> appSettings, KafkaPubSub kafka, ILogger<NumberPublisher> logger)
    {
        _kafka = kafka;
        _logger = logger;
        _appSettings = appSettings.Value;
    }
    
    public async Task StartAsync()
    {
        var random = new Random();
        var producer = _kafka.GetProducer<Null, string>();
        while (true)
        {
            var num = random.Next(1, _appSettings.MaxNumber);
            var message = JsonConvert.SerializeObject(new QuestionMessage() { Question = num.ToString() });
            await producer.ProduceAsync(Constants.Kafka.TopicName, new Message<Null, string>(){ Value = message });
            _logger.LogInformation(message);
            await Task.Delay(1000);
        }
    }
}