namespace Shared.Saga.Models
{
    public interface ICheckOrderStatus
    {
        Guid CorrelationId { get; }
    }
}
