using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Saga.AllocationStateMachine.Models
{
    public interface IAllocationCreated
    {
        Guid AllocationId { get; }

        /// <summary>
        /// The duration when is used to how long hold this allocation, and after that the allocation should be released
        /// </summary>
        TimeSpan HoldDuration { get; }
    }
}
