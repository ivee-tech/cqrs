using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cqrs.Common.ApiClient.MetadataApi.Models;
using Cqrs.Common.Logging;

namespace Cqrs.Common.ApiClient.MetadataApi
{
    public class MetadataApiClient : ApiClientBase,IMetadataApiClient
    {
        private const string _appMetadataPath = "api/metadata/app";

        public MetadataApiClient(HttpClient httpClient, ILoggingService<ApiClientBase> logger) 
            : base(httpClient, logger)
        {
        }

        public async Task<AppMetadataDto> GetAppMetadata(Guid appId)
        {
            var path = $"{_appMetadataPath}/{appId}/";

            var response = await GetAsync<AppMetadataDto>(path);
            EnsureDependencyServiceSuccess(response);

            return response.Result;
        }
    }
}
