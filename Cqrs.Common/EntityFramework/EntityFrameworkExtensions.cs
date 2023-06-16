using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cqrs.Common.EntityFramework
{
    public static class EntityFrameworkExtensions
    {
        public static IQueryable<T> GetPage<T>(this IQueryable<T> source, int pageIndex, int pageSize, out int totalCount, out int totalPages)
        {
            totalCount = source.Count();
            totalPages = 0;

            var results = new List<T>();

            if (totalCount > 0)
            {
                if (pageSize > 0)
                {
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                    if (pageIndex > 0)
                    {
                        source = source.Skip(pageIndex * pageSize);
                    }
                    source = source.Take(pageSize);
                }
                results = source.ToList();
            }

            return results.AsQueryable();
        }

        public static IQueryable<T> If<T>(
            this IQueryable<T> source,
            bool condition,
            Func<IQueryable<T>, IQueryable<T>> transform
        )
        {
            return condition ? transform(source) : source;
        }

        public static IQueryable<T> If<T, P>(
            this IIncludableQueryable<T, P> source,
            bool condition,
            Func<IIncludableQueryable<T, P>, IQueryable<T>> transform
        )
            where T : class
        {
            return condition ? transform(source) : source;
        }

        public static IQueryable<T> If<T, P>(
            this IIncludableQueryable<T, IEnumerable<P>> source,
            bool condition,
            Func<IIncludableQueryable<T, IEnumerable<P>>, IQueryable<T>> transform
        )
            where T : class
        {
            return condition ? transform(source) : source;
        }
    }
}
