using CusomerManagement.Infra.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace CusomerManagement.Persistance
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
