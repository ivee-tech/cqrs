using System;
using Cqrs.Common.Constants;

namespace Cqrs.Common.Exceptions
{
    public class NotFoundException : ExceptionBase
    {
        public NotFoundException(string fieldName, object fieldValue, Exception exception = null)
            : base(null, "Not Found Exception", ApiCode.NotFound, $"{fieldName} ({fieldValue}) was not found.", exception)
        {
        }
    }
}
