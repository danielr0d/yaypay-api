using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Yaypay.Domain;

namespace Yaypay.Worker;

public class PaymentRequestedConsumer : IConsumer<PaymentResquestedEvent>
{
    private readonly IMongoCollection<PaymentRecord> _collection;
    private readonly ILogger<PaymentRequestedConsumer> _logger;

    public PaymentRequestedConsumer(IMongoCollection<PaymentRecord> collection, ILogger<PaymentRequestedConsumer> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentResquestedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Iniciando processamento do pagamento: {PaymentId} no valor de {Amount}", message.PaymentId, message.Amount);

        await Task.Delay(5000); 

        var newStatus = message.Amount > 1000m ? PaymentStatus.Rejected : PaymentStatus.Approved;

        var update = Builders<PaymentRecord>.Update.Set(p => p.Status, newStatus);
        await _collection.UpdateOneAsync(p => p.Id == message.PaymentId, update);

        _logger.LogInformation("Pagamento {PaymentId} finalizado com status: {Status}", message.PaymentId, newStatus);
    }
}