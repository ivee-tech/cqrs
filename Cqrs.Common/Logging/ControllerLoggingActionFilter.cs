using Microsoft.AspNetCore.Mvc.Filters;
using Cqrs.Common.Interfaces;

namespace Cqrs.Common.Logging
{
    public class ControllerLoggingActionFilter : IActionFilter
    {
        private readonly ILoggingService<ControllerLoggingActionFilter> _logger;
        private readonly ICurrentStateService _currentStateService;

        public ControllerLoggingActionFilter(ILoggingService<ControllerLoggingActionFilter> logger,
            ICurrentStateService currentStateService)
        {
            _logger = logger;
            _currentStateService = currentStateService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogDebug($"Executing {context.ActionDescriptor.DisplayName} by {_currentStateService.User}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogDebug($"Executed {context.ActionDescriptor.DisplayName} by {_currentStateService.User}");
        }
    }
}
