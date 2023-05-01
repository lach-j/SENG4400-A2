using A2.Server.Configuration;
using A2.Shared;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace A2.Server;

public class GenerateNumberJob : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly ILogger<GenerateNumberJob> _logger;
    private readonly AppSettings _appSettings;
    private readonly Random _random = new();
    private readonly ServiceBusSender _sender;

    public GenerateNumberJob(ILogger<GenerateNumberJob> logger, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _appSettings = appSettings.Value;

        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        var client = new ServiceBusClient(_appSettings.ServiceBusConnectionString, clientOptions);
        _sender = client.CreateSender(Constants.ServiceBus.QueueName);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(GenerateNumber, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(_appSettings.DelayInterval));
        return Task.CompletedTask;
    }

    private async void GenerateNumber(object? state)
    {
        var num = _random.Next(1, _appSettings.MaxNumber);
        var message = JsonConvert.SerializeObject(new QuestionMessage() { Question = num.ToString() });
        await _sender.SendMessageAsync(new ServiceBusMessage(message));
        _logger.LogInformation(message);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}