using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Common.Constants;
using Cqrs.Common.Interfaces;
using Cqrs.Common.Utilities;

namespace Cqrs.Common.ApiClient
{
    public class ApiClientHandlerMiddleware : DelegatingHandler
    {
        private readonly ICurrentStateService _currentState;

        public ApiClientHandlerMiddleware(ICurrentStateService currentState)
        {
            _currentState = currentState;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ApiHelper.AddRequestHeader(request.Headers, HeaderName.TransactionId, _currentState.TransactionId);
            ApiHelper.AddRequestHeader(request.Headers, HeaderName.User, _currentState.User);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
