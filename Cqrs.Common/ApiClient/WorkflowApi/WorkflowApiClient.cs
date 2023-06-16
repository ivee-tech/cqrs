using System.Net.Http;
using Cqrs.Common.Logging;

namespace Cqrs.Common.ApiClient.WorkflowApi
{
    public class WorkflowApiClient : ApiClientBase, IWorkflowApiClient
    {
        public WorkflowApiClient(HttpClient httpClient, ILoggingService<ApiClientBase> logger) 
            : base(httpClient, logger)
        {
        }
    }
}
