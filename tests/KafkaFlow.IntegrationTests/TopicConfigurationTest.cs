using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaFlow.IntegrationTests.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KafkaFlow.IntegrationTests;

[TestClass]
public class TopicConfigurationTest
{
    private IAdminClient _adminClient;

    [TestInitialize]
    public void Setup()
    {
        _adminClient = Bootstrapper.GetServiceProvider().GetRequiredService<AdminClient>().Client;
    }
    
    [TestMethod]
    public async Task DefaultConfigurationTest()
    {
        var configResource = new ConfigResource
        {
            Type = ResourceType.Topic,
            Name = Bootstrapper.DefaultParamsTopicName
        };
        
        var configs = await _adminClient.DescribeConfigsAsync(new[] { configResource });
        var config = configs.First();
        
        foreach (var key in config.Entries.Keys)
        {
            Assert.IsTrue(config.Entries[key].IsDefault);
        }
    }
    
    [TestMethod]
    public async Task ModifiedConfigurationTest()
    {
        var configResource = new ConfigResource
        {
            Type = ResourceType.Topic,
            Name = Bootstrapper.ModifiedConfigTopicName
        };
        
        var configs = await _adminClient.DescribeConfigsAsync(new[] { configResource });
        var config = configs.First();
        foreach (var key in config.Entries.Keys)
        {
            if (Bootstrapper.s_modifiedConfigValues.TryGetValue(key, out var expectedValue))
            {
                Assert.IsFalse(config.Entries[key].IsDefault);
                Assert.AreEqual(expectedValue, config.Entries[key].Value);
            }
            else
            {
                Assert.IsTrue(config.Entries[key].IsDefault);
            }
        }
    }
}