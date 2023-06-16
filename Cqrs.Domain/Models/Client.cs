using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cqrs.Common.EntityFramework;
using Cqrs.Common.Models;

namespace Cqrs.Domain.Models
{
    public class Client : AuditableBase
    {
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string FamilyName { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
    }
}
