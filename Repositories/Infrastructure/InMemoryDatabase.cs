using Microsoft.EntityFrameworkCore;
using Repositories.Infrastructure.ValueConverters;
using Repositories.Models;

namespace Repositories.Infrastructure
{
    public class InMemoryDatabase : DbContext
    {
        public DbSet<SortJobDto> SortJobTabel { get; init; }

        public InMemoryDatabase(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder
                .Entity<SortJobDto>()
                .Property(e => e.SortedNumbers)
                .HasConversion(new IntegersToStringValueConverter());
        }
    }
}