using Shared;
using MassTransit;

namespace Stock
{
    public class OrderValidateConsumer : IConsumer<IOrderStartedEvent>
    {
        private readonly ILogger<OrderValidateConsumer> _logget;

        public OrderValidateConsumer(ILogger<OrderValidateConsumer> logget)
        {
            _logget = logget;
        }

        public async Task Consume(ConsumeContext<IOrderStartedEvent> context)
        {
            var data = context.Message;
            _logget.LogInformation($"message recieved, {data.ProductName}");
            if (data.PaymentCardNumber.Contains("test"))
            {
                await context.Publish<IOrderCancelEvent>(
                     new
                     {
                         OrderId = context.Message.OrderId,
                         PaymentCardNumber = context.Message.PaymentCardNumber
                     });
            }
            else
            {
                // send to next microservice
            }

        }
    }
}
