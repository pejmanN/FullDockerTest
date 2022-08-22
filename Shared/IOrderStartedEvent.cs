using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public interface IOrderStartedEvent
    {
         Guid OrderId { get; set; }
         string PaymentCardNumber { get; set; }
         string ProductName { get; set; }
    }
}
