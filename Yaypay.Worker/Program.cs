using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Yaypay.Domain;
using Yaypay.Worker;

var builder = Host.CreateApplicationBuilder(args);

var mongoClient = new MongoClient("mongodb://localhost:27017");
var database = mongoClient.GetDatabase("yaypay");   
var paymentsCollection = database.GetCollection<PaymentRecord>("payments");
builder.Services.AddSingleton(paymentsCollection);  

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentRequestedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context); 
    });
});

var host = builder.Build();
await host.RunAsync();
