using MediatR;
using BluePrism.Kafka.Avro.Serializers;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using Confluent.Kafka;
using EventBroker.EventTypes;
using EventBroker.Helpers;

namespace CustomerCommandService.Handlers;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, CreateCustomerResponse>
{
    private string topicNameToPublish = "Customers";

    private static readonly MultipleTypeConfig multipleTypeConfig = new MultipleTypeConfigBuilder()
        .AddType<CustomerCreated>(CustomerCreated._SCHEMA)
        .Build();
    private readonly IEventBrokerProducer eventBrokerProducer;

    public CreateCustomerHandler(IEventBrokerProducer eventBrokerProducer)
    {
        this.eventBrokerProducer = eventBrokerProducer;
    }

    public async Task<CreateCustomerResponse> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        await eventBrokerProducer.Publish<CustomerCreated>(
            new CustomerCreated()
           {
            Id = request.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedDate = DateTime.Now.Ticks
        },
            request.Id.ToString(),
            multipleTypeConfig,
            topicNameToPublish,
            cancellationToken);

        return new CreateCustomerResponse()
        {
            Id = request.Id,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
    }
}

