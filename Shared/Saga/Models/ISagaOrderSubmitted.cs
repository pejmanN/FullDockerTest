using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Saga
{
    public interface ISagaOrderSubmitted
    {
        Guid OrderId { get; set; }

        Guid CorrelationId { get; set; }
        Guid UserId { get; set; }
        string PaymentCardNumber { get; set; }
        string ProductName { get; set; }
    }
}
