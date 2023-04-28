namespace A2.Server.Configuration;

public class AppSettings
{
    public int MaxNumber { get; set; } = 1000000;
    public int DelayInterval { get; set; } = 1000;
    public string ServiceBusConnectionString { get; set; } = "";
}