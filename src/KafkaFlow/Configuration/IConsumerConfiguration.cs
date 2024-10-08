using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaFlow.Configuration;

/// <summary>
/// Represents the Consumer configuration values
/// </summary>
public interface IConsumerConfiguration
{
    /// <summary>
    /// Gets the consumer worker distribution strategy
    /// </summary>
    Factory<IWorkerDistributionStrategy> DistributionStrategyFactory { get; }

    /// <summary>
    /// Gets the consumer middlewares configurations
    /// </summary>
    IReadOnlyList<MiddlewareConfiguration> MiddlewaresConfigurations { get; }

    /// <summary>
    /// Gets the consumer configured topics
    /// </summary>
    IReadOnlyList<string> Topics { get; }

    /// <summary>
    /// Gets the topic partitions to manually assign
    /// </summary>
    IReadOnlyList<TopicPartitions> ManualAssignPartitions { get; }

    /// <summary>
    /// Gets the topic partition offsets to manually assign
    /// </summary>
    IReadOnlyList<TopicPartitionOffsets> ManualAssignPartitionOffsets { get; }

    /// <summary>
    /// Gets the consumer name
    /// </summary>
    string ConsumerName { get; }

    /// <summary>
    /// Gets the cluster configuration
    /// </summary>
    ClusterConfiguration ClusterConfiguration { get; }

    /// <summary>
    /// Gets a value indicating whether the consumer is able to be manageable or not
    /// </summary>
    bool ManagementDisabled { get; }

    /// <summary>
    /// Gets or sets the workers count calculator
    /// </summary>
    Func<WorkersCountContext, IDependencyResolver, Task<int>> WorkersCountCalculator { get; set; }

    /// <summary>
    /// Gets the time interval at which the workers count calculation is re-evaluated.
    /// </summary>
    TimeSpan WorkersCountEvaluationInterval { get; }

    /// <summary>
    /// Gets the consumer group
    /// </summary>
    string GroupId { get; }

    /// <summary>
    /// Gets the buffer size used for each worker
    /// </summary>
    int BufferSize { get; }

    /// <summary>
    /// Gets the time that the worker will wait to process the buffered messages
    /// before canceling the <see cref="IConsumerContext.WorkerStopped"/>
    /// </summary>
    TimeSpan WorkerStopTimeout { get; }

    /// <summary>
    /// Gets a value indicating whether if the application should manual complete the message at the end
    /// </summary>
    bool AutoMessageCompletion { get; }

    /// <summary>
    /// Gets a value indicating whether gets a value indicating that no offsets will be stored on Kafka
    /// </summary>
    bool NoStoreOffsets { get; }

    /// <summary>
    /// Gets the interval between commits
    /// </summary>
    TimeSpan AutoCommitInterval { get; }

    /// <summary>
    /// Gets the handlers used to collects statistics
    /// </summary>
    IReadOnlyList<Action<string>> StatisticsHandlers { get; }

    /// <summary>
    /// Gets the handlers that will be called when the partitions are assigned
    /// </summary>
    IReadOnlyList<Action<IDependencyResolver, List<Confluent.Kafka.TopicPartition>>> PartitionsAssignedHandlers { get; }

    /// <summary>
    /// Gets the handlers that will be called when the partitions are revoked
    /// </summary>
    IReadOnlyList<Action<IDependencyResolver, List<Confluent.Kafka.TopicPartitionOffset>>> PartitionsRevokedHandlers { get; }

    /// <summary>
    /// Gets the handlers that will be called when there are pending offsets
    /// </summary>
    IReadOnlyList<PendingOffsetsStatisticsHandler> PendingOffsetsStatisticsHandlers { get; }

    /// <summary>
    /// Gets the custom factory used to create a new <see cref="KafkaFlow.Consumers.IConsumer"/>
    /// </summary>
    ConsumerCustomFactory CustomFactory { get; }

    /// <summary>
    /// Gets the consumer initial state
    /// </summary>
    ConsumerInitialState InitialState { get; }

    /// <summary>
    /// Parses KafkaFlow configuration to Confluent configuration
    /// </summary>
    /// <returns></returns>
    Confluent.Kafka.ConsumerConfig GetKafkaConfig();
}
