using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cqrs.Common.Models;
using Cqrs.Domain.Comparers;

namespace Cqrs.Domain.Helpers
{
    public static class CompareListsHelper
    {
        public static Tuple<IEnumerable<T>, IEnumerable<T>, IEnumerable<T>> CompareById<T>(IEnumerable<T> src, IEnumerable<T> dest)
            where T : AuditableBase
        {
            var comp = new ModelIdComparer<T>();
            var newItems = src.Except(dest, comp);
            var updatedItems = src.Intersect(dest, comp);
            var deletedItems = dest.Except(src, comp);
            var result = new Tuple<IEnumerable<T>, IEnumerable<T>, IEnumerable<T>>(newItems, updatedItems, deletedItems);
            return result;
        }
    }

}
