using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Saga.AllocationStateMachine;

namespace Warehaous.Infra.Persistence.Configuration
{
    public class AllocationStateMap : SagaClassMap<AllocationState>
    {
        protected override void Configure(EntityTypeBuilder<AllocationState> entity, ModelBuilder model)
        {
            entity.Property(x => x.Version).IsRowVersion();
        }
    }
}
