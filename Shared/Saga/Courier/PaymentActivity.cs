namespace Shared.Saga.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Courier;
    using Shared.Saga.Models;

    public class PaymentActivity : IActivity<IPaymentArguments, IPaymentLog>
    {
        static readonly Random _random = new Random();

        public async Task<ExecutionResult> Execute(ExecuteContext<IPaymentArguments> context)
        {
            string cardNumber = context.Arguments.CardNumber;
            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentNullException(nameof(cardNumber));

            await Task.Delay(300);
            //await Task.Delay(_random.Next(10000));

            if (cardNumber.StartsWith("5999"))
            {
                throw new InvalidOperationException("The card number was invalid");
            }

            return context.Completed(new { AuthorizationCode = "77777" });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<IPaymentLog> context)
        {
            await Task.Delay(100);

            return context.Compensated();
        }
    }
}