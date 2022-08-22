namespace Shared.Saga.Models
{
    public interface IPaymentArguments
    {
        Guid OrderId { get; }
        decimal Amount { get; }
        string CardNumber { get; }
    }

    public interface IPaymentLog
    {
        string AuthorizationCode { get; }

    }
}
