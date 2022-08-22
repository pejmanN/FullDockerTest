using MassTransit;
using Shared;

namespace FullDockerTest.Infra.Consumers
{
    public class StartOrderConsumer : IConsumer<IStartOrder>
    {
        readonly ILogger<StartOrderConsumer> _logger;

        public StartOrderConsumer()
        { }
        public StartOrderConsumer(ILogger<StartOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IStartOrder> context)
        {
            _logger.LogInformation("pejmannn--StartOrderConsumer-- Order" +
                " Transation Started and event published: {OrderId}", context.Message.OrderId);

            await context.Publish<IOrderStartedEvent>(new
            {
                context.Message.OrderId,
                context.Message.PaymentCardNumber,
                context.Message.ProductName
            });

        }
    }
}
