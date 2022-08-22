using Automatonymous;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Saga.Models;
using Shared.Saga.OrderStateMachineActivities;

namespace Shared.Saga
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine(ILogger<OrderStateMachine> logger)
        {
            this.InstanceState(x => x.CurrentState);
            this.ConfigureCorrelationIds();
            Initially(
                When(OrderSubmitted)
                    .Then(x => x.Saga.OrderId = x.Message.OrderId)
                    .Then(x => logger.LogInformation($"Order {x.Saga.OrderId} submitted"))
                    .ThenAsync(c => CusomerValidationCommand(c))
                    .TransitionTo(Submitted)
                );

            During(Submitted,
                Ignore(OrderSubmitted),

                //actually when customer get deactivated we should act different depending upon the order state
                //for the `Submitted` state, transition to `Canceled` is enough
                When(CustomerDeactivated).TransitionTo(Canceled),
                When(OrderAccepted)
                  .Then(x =>
                  {
                      x.Saga.OrderId = x.Message.OrderId;
                  })
                .Activity(x => x.OfType<OrderAcceptedActivity>().TransitionTo(Accepted)));


            During(Accepted,
                When(OrderFulfillmentFaulted).Then(x => { var t = x.Message.OrderId; }).TransitionTo(Faulted),
                When(FulfillOrderFaulted)
                    .Then(x =>
                    {
                        var t = x.Message.Message.CorrelationId;
                        logger.LogInformation($"FulfillOrderConsumer Faulted, message {x.Message.Exceptions.FirstOrDefault()?.Message}");
                    })
                    .TransitionTo(Faulted));


            //initial=0, final=1, and duringAny includes all states except initial and final
            DuringAny(
                When(OrderSubmitted).Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CorrelationId = context.Message.CorrelationId;
                    })
                );

            DuringAny(
               When(OrderStatusRequested)
                   .RespondAsync(x => x.Init<IOrderStatus>(new
                   {
                       CorrelationId = x.Saga.CorrelationId,
                       OrderId = x.Saga.OrderId,
                       Status = x.Saga.CurrentState
                   }))
           );

        }

        private async Task CusomerValidationCommand(BehaviorContext<OrderState, ISagaOrderSubmitted> context)
        {
            var sendEndpoint = await context.GetSendEndpoint(new Uri("queue:" + BusConstants.SagaCusomerValidateQueue));
            await sendEndpoint.Send<ISagaCustomerValidateMessage>(new
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = context.Message.OrderId,
                UserId = context.Message.UserId,
                PaymentCardNumber = context.Message.PaymentCardNumber,
                ProductName = context.Message.ProductName,
            });
        }

        private void ConfigureCorrelationIds()
        {

            Event(() => OrderSubmitted, x => x.CorrelateById(x => x.Message.CorrelationId));
            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.CorrelationId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {

                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<IOrderNotFound>(new { context.Message.CorrelationId });
                    }
                }));
            });

            Event(() => CustomerDeactivated, x => x.CorrelateBy((saga, context) => saga.CustomerId == context.Message.CustomerId));
            Event(() => OrderAccepted, x => x.CorrelateById(x => x.Message.CorrelationId));
            Event(() => OrderFulfillmentFaulted, x => x.CorrelateById(x => x.Message.CorrelationId));
            Event(() => FulfillOrderFaulted, x => x.CorrelateById(x => x.Message.Message.CorrelationId));

        }
        public State Submitted { get; private set; }
        public State Canceled { get; private set; }
        public State Accepted { get; private set; }
        public State Faulted { get; private set; }




        public Event<ISagaOrderSubmitted> OrderSubmitted { get; private set; }
        public Event<ICheckOrderStatus> OrderStatusRequested { get; private set; }
        public Event<ICustomerDeactivated> CustomerDeactivated { get; private set; }
        public Event<IOrderAccepted> OrderAccepted { get; private set; }
        public Event<IOrderFulfillmentFaulted> OrderFulfillmentFaulted { get; private set; }

        public Event<Fault<IFulfillOrder>> FulfillOrderFaulted { get; private set; }

    }
}
