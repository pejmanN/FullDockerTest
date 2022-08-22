using FullDockerTest.Infra.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace FullDockerTest.Infra.Persistence
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}
