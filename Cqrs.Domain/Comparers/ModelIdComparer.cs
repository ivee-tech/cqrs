using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cqrs.Common.Models;

namespace Cqrs.Domain.Comparers
{
    public class ModelIdComparer<T> : IEqualityComparer<T>
        where T : AuditableBase
    {
        public bool Equals(T x, T y)
        {
            if (x == null && y == null)
            {
                return false;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
