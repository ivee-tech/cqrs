using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cqrs.Common.Data
{
    public class DataFilter
    {
        public IEnumerable<DataSort> Sort { get; set; } = new List<DataSort>();
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public FilterValueItem Filter { get; set; }

        public IEnumerable<string> ProjectionFields { get; set; } = new List<string>();
    }

    public class DataSort
    {
        public string Name { get; set; }
        public SortType? SortType { get; set; }
    }

    public enum SortType
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    }
}
