using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using EventBroker.EventTypes;
using BluePrism.Kafka.Avro.Serializers;
using Confluent.SchemaRegistry.Serdes;
namespace EventBroker.Helpers;

public class EventBrokerConsumerFactory : IEventBrokerConsumerFactory
{
    private static string schemaRegistryUrl = "http://schema-registry:8081";
    private static string bootstrapServers = "kafka:9092";
    public IConsumer<string, object> Create(string topicNameToConsume, MultipleTypeConfig multipleTypeConfig)
    {

        var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaRegistryUrl });

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "1",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true
        };

        return new ConsumerBuilder<string, object>(consumerConfig)
                .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                .SetValueDeserializer(new MultipleTypeDeserializer<object>(multipleTypeConfig, schemaRegistry).AsSyncOverAsync())
                .Build();
    }
}
