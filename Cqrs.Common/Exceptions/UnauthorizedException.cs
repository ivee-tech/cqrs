using System;
using Cqrs.Common.Constants;

namespace Cqrs.Common.Exceptions
{
    public class UnauthorizedException : ExceptionBase
    {
        public UnauthorizedException(string resource, Exception exception = null)
            : base(null, "Unauthorized Exception", ApiCode.Unauthorized, $"Unauthorized access to {resource} resource.", exception)
        {
        }
    }
}
