using MassTransit;
using Shared.Saga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Saga.Courier
{
    //In RabbitMq for each Courier Activity two queues will be created, one for Exceute (allocate-inventory_execute)
    //  and another for Compensation (allocate-inventory_compensate)
    public class AllocateInventoryActivity : IActivity<IAllocateInventoryArguments, IAllocateInventoryLog>
    {
        readonly IRequestClient<IAllocateInventory> _client;

        public AllocateInventoryActivity(IRequestClient<IAllocateInventory> client)
        {
            _client = client;
        }
        public async Task<ExecutionResult> Execute(ExecuteContext<IAllocateInventoryArguments> context)
        {
            var orderId = context.Arguments.OrderId;

            var itemNumber = context.Arguments.ItemNumber;
            if (string.IsNullOrEmpty(itemNumber))
                throw new ArgumentNullException(nameof(itemNumber));

            var quantity = context.Arguments.Quantity;
            if (quantity <= 0.0m)
                throw new ArgumentNullException(nameof(quantity));

            var allocationId = NewId.NextGuid();

            var response = await _client.GetResponse<IInventoryAllocated>(new
            {
                AllocationId = allocationId,
                ItemNumber = itemNumber,
                Quantity = quantity
            });

            return context.Completed(new { AllocationId = allocationId });
        }
        public async Task<CompensationResult> Compensate(CompensateContext<IAllocateInventoryLog> context)
        {
            await context.Publish<IAllocationReleaseRequested>(new
            {
                context.Log.AllocationId,
                Reason = "Order Faulted"
            });

            return context.Compensated();
        }

    }
}
