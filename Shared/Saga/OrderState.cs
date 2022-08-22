using Automatonymous;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Saga
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }




        // If using Optimistic concurrency, this property is required
        public byte[] RowVersion { get; set; }
    }
}
