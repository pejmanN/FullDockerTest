namespace Shared.Saga.Models
{
    public interface IFulfillOrder
    {
        Guid OrderId { get; }
        Guid CorrelationId { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
    }
}
