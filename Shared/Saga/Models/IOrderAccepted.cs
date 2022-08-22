namespace Shared.Saga.Models
{
    public interface IOrderAccepted
    {
        Guid OrderId { get; set; }

        Guid CorrelationId { get; set; }
        Guid UserId { get; set; }
        string PaymentCardNumber { get; set; }
        string ProductName { get; set; }
    }
}
