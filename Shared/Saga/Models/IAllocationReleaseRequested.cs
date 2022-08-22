namespace Shared.Saga.Models
{
    //p
    public interface IAllocationReleaseRequested
    {
        Guid AllocationId { get; }
        string Reason { get; }
    }
}
