using MassTransit;

namespace Shared.Saga.AllocationStateMachine
{
    public class AllocationState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        /// <summary>
        /// when a message is scheduled in MassTransit, it consider a GUID for that message, by using this GUID we can access to related schedule
        /// and we can for example cancel the schedule
        /// </summary>
        public Guid? HoldDurationToken { get; set; }

        public byte[] Version { get; set; }
    }
}
