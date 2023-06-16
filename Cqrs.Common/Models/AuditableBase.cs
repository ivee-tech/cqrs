using System;

namespace Cqrs.Common.Models
{
    public class AuditableBase
    {
        public Guid? Id { get; set; }

        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? Updated { get; set; }
    }
}
