using A2.Server.Services;
using A2.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(options => builder.Configuration.GetSection("AppSettings").Bind(options));

builder.Services.AddSingleton<KafkaPubSub>();
builder.Services.AddSingleton<NumberPublisher>();
var app = builder.Build();

var config = app.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

var myService = app.Services.GetService<NumberPublisher>();
Task.Run(async () =>
{
    await myService?.StartAsync()!;
}).Wait();

app.Run();