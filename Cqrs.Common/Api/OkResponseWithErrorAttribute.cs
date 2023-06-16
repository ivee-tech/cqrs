using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Cqrs.Common.Exceptions;

namespace Cqrs.Common.Api
{
    public class OkResponseWithErrorAttribute : IExceptionFilter
    {
        private readonly string _code;
        private readonly string _message;

        public OkResponseWithErrorAttribute()
        {
            _code = StatusCodes.Status500InternalServerError.ToString();
            _message = string.Empty;
        }

        public OkResponseWithErrorAttribute(string code, string message)
        {
            _code = code;
            _message = message;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is HandledException)
            {
            }
            else
            {
                throw new HandledException(_code,
                    !string.IsNullOrEmpty(_message) ? _message : context.Exception.Message,
                    context.Exception);
            }
        }
    }
}
