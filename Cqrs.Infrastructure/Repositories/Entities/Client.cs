using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cqrs.Common.EntityFramework;

namespace Cqrs.Infrastructure.Repositories.Entities
{
    public class Client : EntityBase
    {
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string FamilyName { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
    }

    public class ClientTypeConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            builder.Property(p => p.FirstName).HasMaxLength(250);
            builder.Property(p => p.MiddleNames).HasMaxLength(250);
            builder.Property(p => p.FamilyName).HasMaxLength(250);
            builder.Property(p => p.DateOfBirth).HasMaxLength(250);
            builder.Property(p => p.Phone).HasMaxLength(50);
            builder.Property(p => p.Email).HasMaxLength(100);

        }
    }
}
