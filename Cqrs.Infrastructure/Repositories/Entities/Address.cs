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
    public class Address : EntityBase
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Code { get; set; }
        public Guid ClientId { get; set; }
        public Client Client { get; set; }
    }

    public class AddressTypeConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.Property(p => p.Line1).HasMaxLength(250);
            builder.Property(p => p.Line2).HasMaxLength(250);
            builder.Property(p => p.Suburb).HasMaxLength(250);
            builder.Property(p => p.State).HasMaxLength(50);
            builder.Property(p => p.Code).HasMaxLength(10);

        }
    }
}
