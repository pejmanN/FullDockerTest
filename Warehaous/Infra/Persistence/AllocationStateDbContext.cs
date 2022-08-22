using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Warehaous.Infra.Persistence.Configuration;

namespace Warehaous.Infra.Persistence
{
    public class AllocationStateDbContext : SagaDbContext
    {
        public AllocationStateDbContext(DbContextOptions<AllocationStateDbContext> options) : base(options)
        { }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new AllocationStateMap(); }
        }
    }
}
