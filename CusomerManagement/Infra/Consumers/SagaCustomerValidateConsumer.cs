using CusomerManagement.Persistance;
using MassTransit;
using Shared.Saga;
using Shared.Saga.Models;

namespace FullDockerTest.Consumers
{
    public class SagaCustomerValidateConsumer : IConsumer<ISagaCustomerValidateMessage>
    {
        private readonly ILogger<SagaCustomerValidateConsumer> _logger;
        private readonly AppDbContext _db;
        public SagaCustomerValidateConsumer()
        { }
        public SagaCustomerValidateConsumer(ILogger<SagaCustomerValidateConsumer> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task Consume(ConsumeContext<ISagaCustomerValidateMessage> context)
        {
            _logger.LogInformation("CustomerValidateConsumer-- Order" +
                " Transation Started and event published: {OrderId}", context.Message.OrderId);

            try
            {

                //var customer = _db.Customers.FirstOrDefault(x => x.Id == context.Message.UserId);
                //if (customer == null)
                //{
                //    throw new InvalidOperationException();
                //}
                //if (!customer.IsActive)
                //{
                //    throw new InvalidOperationException();
                //}

                //_logger.LogInformation("customer with  name {Name}", customer.Name);

                await context.Publish<IOrderAccepted>(new
                {
                    OrderId = context.Message.OrderId,
                    UserId = context.Message.UserId,
                    PaymentCardNumber = context.Message.PaymentCardNumber,
                    ProductName = context.Message.ProductName,
                    CorrelationId = context.Message.CorrelationId
                });


            }
            catch (Exception ex)
            {
                _logger.LogError("error occurd", ex);
                throw new InvalidOperationException();
            }
        }
    }
}
