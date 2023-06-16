using Microsoft.EntityFrameworkCore;
using System.Linq;
using Cqrs.Common.EntityFramework;
using Cqrs.Common.Interfaces;
using Cqrs.Infrastructure.Repositories.Entities;
using e = Cqrs.Infrastructure.Repositories.Entities;

namespace Cqrs.Infrastructure.Repositories.DbContexts
{
    public class DataDbContext : DbContextBase
    {

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Client> Clients { get; set; }


        public DataDbContext(DbContextOptions<DataDbContext> options, ICurrentStateService currentStateService)
                    : base(options, currentStateService)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AddressTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientTypeConfiguration());

            ConfigureForeignKeys(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureForeignKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
