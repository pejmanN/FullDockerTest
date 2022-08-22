using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Saga
{
    public interface ISagaStartOrder
    {
        Guid OrderId { get; set; }
        Guid UserId { get; set; }
        string PaymentCardNumber { get; set; }
        string ProductName { get; set; }
    }
}
