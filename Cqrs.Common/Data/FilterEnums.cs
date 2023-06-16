using System;
using System.Collections.Generic;
using System.Text;

namespace Cqrs.Common.Data
{

    public enum ComparisonOperator
    {
        Equal = 0,
        GreaterThan = 1,
        GreaterOrEqualThan = 2,
        LessThan = 3,
        LessOrEqualThan = 4,
        Between = 5,
        IsNull = 6,
        Contains = 7,
        NotEqual = 8,
        StartsWith = 9,
        EndsWith = 10,
        IsNotNull = 12
    }

    public enum BinaryOperator
    {
        None = 0,
        And = 1,
        Or = 2
    }
}
