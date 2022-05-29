using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using EventBroker.EventTypes;
using EventBroker.Helpers;
using BluePrism.Kafka.Avro.Serializers;
using Confluent.SchemaRegistry.Serdes;

namespace CustomerCommandService.EventBroker.Consumers
{
    public class CreateCustomerSagaConsumer : BackgroundService
    {
        private string topicNameToConsume = "Customers";
        private static readonly MultipleTypeConfig multipleTypeConfig = new MultipleTypeConfigBuilder()
           .AddType<EnvironmentCreated>(EnvironmentCreated._SCHEMA)
           .AddType<EnvironmentCreateFailed>(EnvironmentCreated._SCHEMA)
           .Build();
        private readonly ILogger<CreateCustomerSagaConsumer> _logger;
         private readonly IEventBrokerConsumerFactory eventBrokerConsumerFactory;

        public CreateCustomerSagaConsumer(IEventBrokerConsumerFactory eventBrokerConsumerFactory, ILogger<CreateCustomerSagaConsumer> logger)
        {
            this._logger = logger;
            this.eventBrokerConsumerFactory = eventBrokerConsumerFactory;
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
                            case EnvironmentCreated requested:
                                
                                break;
                            case EnvironmentCreateFailed started:
                                
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
    }
}
