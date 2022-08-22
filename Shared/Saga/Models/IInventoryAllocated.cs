namespace Shared.Saga.Models
{
    public interface IInventoryAllocated
    {
        Guid AllocationId { get; }

        string ItemNumber { get; }
        decimal Quantity { get; }
    }
}
