using A2.Server;
using A2.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(options => builder.Configuration.GetSection("AppSettings").Bind(options));

builder.Services.AddHostedService<GenerateNumberJob>();
var app = builder.Build();

app.Run();