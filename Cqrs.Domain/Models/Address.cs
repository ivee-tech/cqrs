﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
    public class Address : AuditableBase
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Code { get; set; }
        public Client Client { get; set; }
    }
}
