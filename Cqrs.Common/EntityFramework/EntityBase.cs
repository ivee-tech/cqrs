using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cqrs.Common.EntityFramework
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string UpdatedBy { get; set; }

        public DateTime? Updated { get; set; }
    }
}
