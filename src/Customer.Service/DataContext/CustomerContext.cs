using CustomerCommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerCommandService.Data
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }

        public DbSet<CustomerCommandService.Models.Customer> Customers { get; set; }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerCommandService.Models.Customer>().ToTable("Customer");            
        }
    }
}