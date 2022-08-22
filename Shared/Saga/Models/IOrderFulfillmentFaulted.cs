using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Saga.Models
{
    public interface IOrderFulfillmentFaulted
    {
        Guid OrderId { get; }

        DateTime Timestamp { get; }
        Guid CorrelationId { get; }
    }
}
