using System;
using System.Threading.Tasks;
using Cqrs.Common.ApiClient.MetadataApi.Models;

namespace Cqrs.Common.ApiClient.MetadataApi
{
    public interface IMetadataApiClient
    {
        Task<AppMetadataDto> GetAppMetadata(Guid appId);
    }
}
