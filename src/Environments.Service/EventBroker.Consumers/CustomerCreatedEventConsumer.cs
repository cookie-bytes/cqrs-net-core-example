using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using EventBroker.EventTypes;
using BluePrism.Kafka.Avro.Serializers;
using Confluent.SchemaRegistry.Serdes;
using EventBroker.Helpers;
namespace EventBroker.Consumers;

public class CustomerCreatedEventConsumer : BackgroundService
{
    private string topicNameToConsume = "Customers";
    private string topicNameToPublish = "Environments";
    private static readonly MultipleTypeConfig multipleTypeConfig = new MultipleTypeConfigBuilder()
       .AddType<CustomerCreated>(CustomerCreated._SCHEMA)
       .AddType<EnvironmentCreated>(EnvironmentCreated._SCHEMA)
       .AddType<EnvironmentCreateFailed>(EnvironmentCreateFailed._SCHEMA)
       .Build();
    private readonly ILogger<CustomerCreatedEventConsumer> _logger;
    private readonly IEventBrokerConsumerFactory eventBrokerConsumerFactory;
    private readonly IEventBrokerProducer eventBrokerProducer;

    public CustomerCreatedEventConsumer(IEventBrokerConsumerFactory eventBrokerConsumerFactory, IEventBrokerProducer eventBrokerProducer, ILogger<CustomerCreatedEventConsumer> logger)
    {
        this._logger = logger;
        this.eventBrokerConsumerFactory = eventBrokerConsumerFactory;
        this.eventBrokerProducer = eventBrokerProducer;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        new Thread(async () => await StartConsumerLoop(stoppingToken)).Start();
        return Task.CompletedTask;
    }

    private async Task StartConsumerLoop(CancellationToken cancellationToken)
    {
        using IConsumer<string, object> consumer = eventBrokerConsumerFactory.Create(topicNameToConsume, multipleTypeConfig);
        consumer.Subscribe(topicNameToConsume);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(cancellationToken);
                var message = consumeResult.Message;
                switch (message.Value)
                {
                    case CustomerCreated customerCreated:
                        await ProcessCustomerCreated(customerCreated, cancellationToken);
                        break;
                }
            }
            catch (OperationCanceledException e)
            {
                _logger.LogError($"Operation Canceled error: {e}");
            }
            catch (ConsumeException e)
            {
                _logger.LogError($"Consume error: {e.Error.Reason}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error: {e}");
            }
        }
    }

    private async Task ProcessCustomerCreated(CustomerCreated customerCreated, CancellationToken cancellationToken)
    {
        if (customerCreated.Id == 100)
        {
            await eventBrokerProducer.Publish<EventBroker.EventTypes.EnvironmentCreateFailed>(
                new EventBroker.EventTypes.EnvironmentCreateFailed()
                {
                    CustomerId = customerCreated.Id,
                    CreatedDate = DateTime.Now.Ticks
                },
                customerCreated.Id.ToString(),
                multipleTypeConfig,
                topicNameToPublish,
                cancellationToken);
        }
        else
        {
            await eventBrokerProducer.Publish<EventBroker.EventTypes.EnvironmentCreated>(
                new EnvironmentCreated()
                {
                    Id = new Random(100000).Next(),
                    CustomerId = customerCreated.Id,
                    CreatedDate = DateTime.Now.Ticks
                },
                customerCreated.Id.ToString(),
                multipleTypeConfig,
                topicNameToPublish,
                cancellationToken);
        }
    }
}
