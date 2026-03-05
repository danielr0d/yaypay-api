namespace Yaypay.Domain;

public enum Models
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
    public Models Status { get; set; } = Models.Pending;
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public record PaymentResquestedEvent(string PaymentId, decimal Amount);