using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cqrs.Common.EntityFramework;

namespace Cqrs.Infrastructure.Repositories.Comparers
{
    public class EntityIdComparer<T> : IEqualityComparer<T>
        where T : EntityBase
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
