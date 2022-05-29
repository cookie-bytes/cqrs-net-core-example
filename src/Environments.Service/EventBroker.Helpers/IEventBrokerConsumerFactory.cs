using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using EventBroker.EventTypes;
using BluePrism.Kafka.Avro.Serializers;
using Confluent.SchemaRegistry.Serdes;
namespace EventBroker.Helpers;

public interface IEventBrokerConsumerFactory
{
    IConsumer<string, object> Create(string topicNameToConsume, MultipleTypeConfig multipleTypeConfig);
}
