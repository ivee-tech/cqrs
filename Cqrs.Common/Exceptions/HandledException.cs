using System;

namespace Cqrs.Common.Exceptions
{
  public class HandledException: ExceptionBase
    {
        public HandledException(string url, string code, string message, Exception exception = null)
            : base(url,  "Handlled Api Exception", code, message, exception)
        {

        }

        public HandledException(string url, string code, Exception exception)
            : this(url, code, exception.Message, exception)
        {
        }
    }
}
