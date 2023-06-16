using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Cqrs.Common.ApiClient.AppDataApi
{
    public interface IAppDataApiClient
    {
        Task<JObject> GetAppData(Guid entityMetadataId, Guid entityId);
    }
}
