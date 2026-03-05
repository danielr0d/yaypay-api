using MassTransit;
using MongoDB.Driver;
using Payment.Domain;
using Payment.Worker;

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
host.Run();