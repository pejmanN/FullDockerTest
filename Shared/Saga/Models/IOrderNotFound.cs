namespace Shared.Saga.Models
{
    public interface IOrderNotFound
    {
        Guid CorrelationId { get; set; }
    }
    public interface IOrderStatus
    {
        public string Status { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid OrderId { get; set; }
    }
}
