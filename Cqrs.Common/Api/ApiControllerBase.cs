using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Cqrs.Common.Exceptions;

namespace Cqrs.Common.Api
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected string CurrentUrl => HttpContext.Request.GetEncodedUrl();

        public ApiControllerBase()
        {
        }

        protected IActionResult BaseOk(object result)
        {
            return base.Ok(result);
        }

        protected IActionResult BaseOk<TResult>(TResult result)
        {
            return base.Ok(result);
        }

        protected IActionResult Ok()
        {
            return base.Ok(new Envelope<object>(CurrentUrl));
        }

        protected IActionResult Ok<TResult>(TResult result)
        {
            return base.Ok(new Envelope<TResult>(CurrentUrl, result));
        }

        protected IActionResult Ok<TResult>(IEnumerable<TResult> results)
        {
            return base.Ok(new Envelope<TResult>(CurrentUrl, results));
        }

        protected IActionResult Ok<TResult>(IEnumerable<TResult> results, int pageIndex, int totalPages, int totalCount)
        {
            return base.Ok(new PaginatedEnvelope<TResult>(CurrentUrl, results, pageIndex, totalPages, totalCount));
        }

        protected IActionResult Error(string code, string message)
        {
            return base.Ok(new Envelope<object>(CurrentUrl, code, message));
        }

        protected IActionResult Error(HandledException exception)
        {
            return base.Ok(new Envelope<object>(exception));
        }
    }
}
