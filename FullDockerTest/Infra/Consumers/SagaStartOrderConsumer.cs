using MassTransit;
using Shared.Saga;

namespace FullDockerTest.Infra.Consumers
{
    public class SagaStartOrderConsumer : IConsumer<ISagaStartOrder>
    {
        private readonly ILogger<SagaStartOrderConsumer> _logger;
        public SagaStartOrderConsumer()
        { }
        public SagaStartOrderConsumer(ILogger<SagaStartOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<ISagaStartOrder> context)
        {
            _logger.LogInformation("pejmannn--ISagaStartOrder-- Order" +
                " Transation Started and event published: {OrderId}", context.Message.OrderId);

            await context.Publish<ISagaOrderSubmitted>(new
            {
                OrderId = context.Message.OrderId,
                UserId = context.Message.UserId,
                PaymentCardNumber = context.Message.PaymentCardNumber,
                ProductName = context.Message.ProductName,
                CorrelationId = Guid.NewGuid()
            });
        }
    }
}
