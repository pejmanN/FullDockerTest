using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Saga.Models
{
    public interface IAllocateInventoryArguments
    {
        Guid OrderId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }
    public interface IAllocateInventoryLog
    {
        Guid AllocationId { get; }
    }

}
