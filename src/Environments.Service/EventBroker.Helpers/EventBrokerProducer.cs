using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using EventBroker.EventTypes;
using BluePrism.Kafka.Avro.Serializers;
using Confluent.SchemaRegistry.Serdes;
namespace EventBroker.Helpers;

public class EventBrokerProducer : IEventBrokerProducer
{
    private string schemaRegistryUrl = "http://schema-registry:8081";
    private string bootstrapServers = "kafka:9092";

    public async Task Publish<T>(T @event, string key, MultipleTypeConfig multipleTypeConfig, string topicNameToPublish, CancellationToken cancellationToken)
    {
        var serializerConfig = new AvroSerializerConfig
        {
            SubjectNameStrategy = SubjectNameStrategy.Record,
            AutoRegisterSchemas = true
        };
        using var schemaRegistryClient = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaRegistryUrl });
        var serializer = new MultipleTypeSerializer<object>(multipleTypeConfig, schemaRegistryClient, serializerConfig);
        using var producer =
                     new ProducerBuilder<string, object>(new ProducerConfig { BootstrapServers = bootstrapServers })
                         .SetKeySerializer(new AvroSerializer<string>(schemaRegistryClient))
                         .SetValueSerializer(serializer)
                         .Build();

        async Task ProduceMessage(string key, object value) => await producer.ProduceAsync(topicNameToPublish,
         new Message<string, object>
         {
             Key = key,
             Value = value
         }, cancellationToken);

        await ProduceMessage(key, @event);
    }
}
