namespace Shared
{
    public interface IOrderCancelEvent
    {
        public Guid OrderId { get; }
        public string PaymentCardNumber { get; }
        public string ProductName { get; }
    }

}
