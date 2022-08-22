using Automatonymous;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Saga.Models;

namespace Shared.Saga.OrderStateMachineActivities
{
    public class OrderAcceptedActivity : IStateMachineActivity<OrderState, IOrderAccepted>
    {
        readonly ILogger<OrderAcceptedActivity> _logger;
        public OrderAcceptedActivity(ILogger<OrderAcceptedActivity> logger)
        {
            _logger = logger;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, IOrderAccepted> context, IBehavior<OrderState, IOrderAccepted> next)
        {


            _logger.LogInformation("call form AcceptOrderActivity");

            //Since it's an StateMachin Activity, it does not know anything about the Masstransit(but masstransit knows about this stateMachin)
            //so in Activity's context we cant do Send action and need the ConsumeContext for acting something like this
            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("queue:fulfill-order"));

            await sendEndpoint.Send<IFulfillOrder>(new
            {
                context.Message.OrderId,
                context.Message.CorrelationId,
                context.Saga.CustomerId,
                PaymentCardNumber = "5099"
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, IOrderAccepted, TException> context, IBehavior<OrderState, IOrderAccepted> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("accept-order");
        }
    }
}
