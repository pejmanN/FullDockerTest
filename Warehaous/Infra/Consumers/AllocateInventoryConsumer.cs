using MassTransit;
using Shared.Saga.AllocationStateMachine.Models;
using Shared.Saga.Models;

namespace Warehaous.Infra.Consumers
{
    public class AllocateInventoryConsumer : IConsumer<IAllocateInventory>
    {
        readonly ILogger<AllocateInventoryConsumer> _logger;
        public AllocateInventoryConsumer(ILogger<AllocateInventoryConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IAllocateInventory> context)
        {
            _logger.LogInformation($"AllocateInventoryConsumer called for AllocationId {context.Message.AllocationId}");

            await context.Publish<IAllocationCreated>(new
            {
                context.Message.AllocationId,
                HoldDuration = 15000,
            });

            await context.RespondAsync<IInventoryAllocated>(new
            {
                AllocationId = context.Message.AllocationId,
                ItemNumber = context.Message.ItemNumber,
                Quantity = context.Message.Quantity,
            });
        }
    }
}
