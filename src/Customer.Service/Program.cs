using CustomerCommandService.Data;
using Microsoft.EntityFrameworkCore;
using CustomerCommandService.Models;
using CustomerCommandService.Requests;
using MediatR;
using System.Reflection;
using CustomerCommandService.Handlers;
using EventBroker.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CustomerContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton<IEventBrokerConsumerFactory, EventBrokerConsumerFactory>();
builder.Services.AddSingleton<IEventBrokerProducer, EventBrokerProducer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




app.MapPost("/customer", async (CustomerCommandService.Requests.CreateCustomerRequest createCustomerRequest, IMediator mediator ) =>
{
    var response = await mediator.Send(new CustomerCommandService.Handlers.CreateCustomerRequest()
    {
        Id = createCustomerRequest.Id,
        FirstName = createCustomerRequest.FirstName,
        LastName = createCustomerRequest.LastName
    });
    return response;
})
.WithName("CreateCustomer");


DataHelper.CreateDbIfNotExists(app.Services);

app.Run();
