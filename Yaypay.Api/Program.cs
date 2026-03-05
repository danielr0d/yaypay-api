using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;   
using Yaypay.Domain;

var builder = WebApplication.CreateBuilder(args);

var mongoClient = new MongoClient("mongodb://localhost:27017");
var database = mongoClient.GetDatabase("yaypay");   
var paymentsCollection = database.GetCollection<PaymentRecord>("payments");
builder.Services.AddSingleton(paymentsCollection);  

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

app.MapPost("/payments", async ([FromBody] PaymentRecord request,
    IMongoCollection<PaymentRecord> collection,
    IPublishEndpoint publishEndpoint) =>
{
    request.Status = PaymentStatus.Pending;
    await collection.InsertOneAsync(request);

    var paymentEvent = new PaymentResquestedEvent(request.Id, request.Amount);
    await publishEndpoint.Publish(paymentEvent);

    return Results.Accepted($"/payments/{request.Id}", request);
});    

app.MapGet("/payments/{id}", async (string id, IMongoCollection<PaymentRecord> collection) =>
{
    var payment = await collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    return payment is not null ? Results.Ok(payment) : Results.NotFound();
});

app.Run();