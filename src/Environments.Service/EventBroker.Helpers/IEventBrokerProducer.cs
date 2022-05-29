using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using EventBroker.EventTypes;
using BluePrism.Kafka.Avro.Serializers;
using Confluent.SchemaRegistry.Serdes;
namespace EventBroker.Helpers;

public interface IEventBrokerProducer
{
    Task Publish<T>(T @event, string key, MultipleTypeConfig multipleTypeConfig, string topicNameToPublish, CancellationToken cancellationToken);
}
