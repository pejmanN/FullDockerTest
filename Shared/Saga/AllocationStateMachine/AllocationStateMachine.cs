using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Saga.AllocationStateMachine.Models;
using Shared.Saga.Models;

namespace Shared.Saga.AllocationStateMachine
{
    public class AllocationStateMachine : MassTransitStateMachine<AllocationState>
    {
        public AllocationStateMachine(ILogger<AllocationStateMachine> logger)
        {

            this.InstanceState(x => x.CurrentState);
            this.ConfigureCorrelationIds();

            Schedule(() => HoldExpiration, (allocationState) => allocationState.HoldDurationToken, (config) =>
            {
                //Set a deafult and fixed message delay
                config.Delay = TimeSpan.FromHours(1);

                config.Received = x => x.CorrelateById(m => m.Message.AllocationId);
            });

            Initially(When(AllocationCreated)
                             .Then(context => logger.LogInformation($"AllocationCreated for AllocationId: {context.Message.AllocationId}"))
                             .Schedule(HoldExpiration,
                                    context =>
                                    {
                                        var msg = context.Init<AllocationHoldDurationExpired>(new { context.Message.AllocationId });
                                        return msg;
                                    },
                                    context => context.Message.HoldDuration)
                            .TransitionTo(Allocated));


            During(Allocated,
              When(HoldExpiration.Received)
                  .Then(context =>
                  {
                      logger.LogInformation("Allocation expired {AllocationId}", context.Saga.CorrelationId);
                  }).Finalize(),

                 When(ReleaseRequested)
                    .Unschedule(HoldExpiration)
                    .Then(context => logger.LogInformation("Allocation Release Granted: {AllocationId}", context.Saga.CorrelationId))
                    .Finalize()
             );


            //     Sets the state machine instance to Completed when in the final state. The saga
            //     repository removes completed state machine instances.

            //SetCompletedWhenFinalized();

        }

        private void ConfigureCorrelationIds()
        {
            Event(() => AllocationCreated, x => x.CorrelateById(m => m.Message.AllocationId));
            Event(() => ReleaseRequested, x => x.CorrelateById(m => m.Message.AllocationId));
        }

        public Schedule<AllocationState, AllocationHoldDurationExpired> HoldExpiration { get; set; }

        public State Allocated { get; set; }
        public State Released { get; set; }

        public Event<IAllocationCreated> AllocationCreated { get; set; }
        public Event<IAllocationReleaseRequested> ReleaseRequested { get; set; }
    }
}
