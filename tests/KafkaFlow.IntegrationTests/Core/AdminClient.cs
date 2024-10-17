using Confluent.Kafka;

namespace KafkaFlow.IntegrationTests.Core;

public class AdminClient
{
    public IAdminClient Client { get; }
    
    public AdminClient(string bootstrapServers)
    {
        Client = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();
    }
}