namespace Shared.Saga.Models
{
    public interface ICustomerDeactivated
    {
        Guid CustomerId { get; set; }
    }
}
