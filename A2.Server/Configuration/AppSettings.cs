namespace A2.Server.Configuration;

public class AppSettings
{
    public AppSettings()
    {
        Kafka = new KafkaSettings();
    }
    public int MaxNumber { get; set; } = 1000000;
    public KafkaSettings Kafka { get; set; }
}

public class KafkaSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}