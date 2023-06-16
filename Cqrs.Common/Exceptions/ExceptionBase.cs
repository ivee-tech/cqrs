using System;

namespace Cqrs.Common.Exceptions
{
    public abstract class ExceptionBase : Exception
    {
        public string Url { get; private set; }
        public string Type { get; private set; }
        public string Code { get; private set; }

        public ExceptionBase(string url, string type, string code, string message, Exception exception)
            : base(message, exception)
        {
            Url = url;
            Type = type;
            Code = code;
        }
    }
}
