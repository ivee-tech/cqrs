using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cqrs.Common.Models;

namespace Cqrs.Common.ApiClient.MetadataApi.Models
{
    public class AppMetadataDto : AuditableBase
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
