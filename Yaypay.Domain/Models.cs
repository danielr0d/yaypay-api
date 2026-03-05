namespace Yaypay.Domain;

public enum PaymentStatus
{
    Pending,
    Approved,
    Rejected,
}

public class PaymentRecord
{
    public string Id { get; set; }  = Guid.NewGuid().ToString();
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty; 
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public record PaymentResquestedEvent(string PaymentId, decimal Amount);