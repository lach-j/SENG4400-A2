using A2.Server.Configuration;
using A2.Shared;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace A2.Server.Services;

public class NumberPublisher
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<NumberPublisher> _logger;

    public NumberPublisher(IOptions<AppSettings> appSettings, ILogger<NumberPublisher> logger)
    {
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    public async Task StartAsync()
    {
        var random = new Random();
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        var client = new ServiceBusClient(_appSettings.ServiceBusConnectionString, clientOptions);
        var sender = client.CreateSender(Constants.ServiceBus.QueueName);
        while (true)
        {
            var num = random.Next(1, _appSettings.MaxNumber);
            var message = JsonConvert.SerializeObject(new QuestionMessage() { Question = num.ToString() });
            await sender.SendMessageAsync(new ServiceBusMessage(message));
            _logger.LogInformation(message);
            await Task.Delay(_appSettings.DelayInterval);
        }
    }
}