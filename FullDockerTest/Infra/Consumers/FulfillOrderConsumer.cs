using MassTransit;
using MassTransit.Courier.Contracts;
using Shared.Saga.Models;

namespace FullDockerTest.Infra.Consumers
{
    public class FulfillOrderConsumer : IConsumer<IFulfillOrder>
    {
        public async Task Consume(ConsumeContext<IFulfillOrder> context)
        {

            if (context.Message.PaymentCardNumber.StartsWith("5099"))
            {
                throw new InvalidDataException("the card number is not valie");
            }


            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            //indeed we have to put `AllocateInventoryActivity`  in warehaouse microservice, and `FulfillOrderConsumer` should be in Order microservice
            //so in builder they use string instead of Type as first argument, it helps  decoupling Microservice 
            builder.AddActivity("AllocateInventory", new Uri("queue:allocate-inventory_execute"), new
            {
                ItemNumber = "ITEM123",
                Quantity = 10.0m
            });


            builder.AddActivity("PaymentActivity", new Uri("queue:payment_execute"),
              new
              {
                  CardNumber = context.Message.PaymentCardNumber,
                  Amount = 99.95m
              });


            //actually with this method we can add `OrderId` to all Activity in routingSlip and we dont need repeat adding repetitive argument
            builder.AddVariable("OrderId", context.Message.OrderId);


            //by this way we send `IOrderFulfillmentFaulted` to OrderStateMachine (when the routingSlip is faulted)
            await builder.AddSubscription(context.SourceAddress,// source address is the sender, here is the OrderStateMachine
                              RoutingSlipEvents.Faulted | RoutingSlipEvents.Supplemental,
                              RoutingSlipEventContents.None,
                              x => x.Send<IOrderFulfillmentFaulted>(new
                              {
                                  context.Message.OrderId,
                                  context.Message.CorrelationId,
                              }));

            var routingSlip = builder.Build();

            await context.Execute(routingSlip);
        }
    }

    public class FulfillOrderConsumerDefinition : ConsumerDefinition<FulfillOrderConsumer>
    {
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<FulfillOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r =>
            {
                //because when this error occred we dont need to retry agian, becuase this error is not such a transient error
                r.Ignore<InvalidDataException>();

                r.Interval(3, 1000);
            });

            endpointConfigurator.DiscardFaultedMessages();
        }
    }
}
