using A2.Server.Configuration;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace A2.Server.Services;

public class KafkaPubSub
{
    private readonly KafkaSettings _appSettings;
    private readonly ProducerConfig _config;
    public KafkaPubSub(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value.Kafka;
        _config = new ProducerConfig
        {
            BootstrapServers = _appSettings.ConnectionString
        };
    }

    public IProducer<TKey, TValue> GetProducer<TKey, TValue>()
    {
        return new ProducerBuilder<TKey, TValue>(_config).Build();
    }
}