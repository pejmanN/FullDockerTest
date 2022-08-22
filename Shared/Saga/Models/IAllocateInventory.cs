namespace Shared.Saga.Models
{
    public interface IAllocateInventory
    {
        Guid AllocationId { get; }

        string ItemNumber { get; }
        decimal Quantity { get; }
    }
}
