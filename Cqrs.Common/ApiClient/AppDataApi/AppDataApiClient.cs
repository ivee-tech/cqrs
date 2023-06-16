using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Cqrs.Common.Logging;

namespace Cqrs.Common.ApiClient.AppDataApi
{
    public class AppDataApiClient : ApiClientBase, IAppDataApiClient
    {
        private const string _appDataPath = "api/appdata/entity";

        public AppDataApiClient(HttpClient httpClient, ILoggingService<ApiClientBase> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<JObject> GetAppData(Guid entityMetadataId, Guid entityId)
        {
            var path = $"{_appDataPath}/{entityMetadataId}/{entityId}";

            var response = await GetAsync<JObject>(path);
            EnsureDependencyServiceSuccess(response);

            return response.Result;
        }
    }
}
