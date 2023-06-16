using Cqrs.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cqrs.Common.EntityFramework
{
    public abstract class DbContextBase : DbContext
    {
        private readonly ICurrentStateService _currentStateService;

        public DbContextBase(DbContextOptions options, ICurrentStateService currentStateService)
            : base(options)
        {
            _currentStateService = currentStateService;
        }
        
        public override int SaveChanges()
        {
            UpdateAuditableFields();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableFields();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditableFields()
        {
            var user = _currentStateService.User;

            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = user;
                        entry.Entity.Created = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = user;
                        entry.Entity.Updated = DateTime.Now;
                        break;
                }
            }
        }
    }
}
