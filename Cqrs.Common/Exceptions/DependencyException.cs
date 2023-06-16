using System;

namespace Cqrs.Common.Exceptions
{
    public class DependencyException: ExceptionBase
    {
        public string Dependency { get; set; }

        public DependencyException(string url, string dependency, string code, string message, Exception exception = null)
            : base(url, "Dependency Exception", code, message, exception)
        {
            Dependency = dependency;
        }

        public DependencyException(string url, string dependency, string code, Exception exception)
            : this(url, dependency, code, exception.Message, exception)
        {
        }
    }
}
