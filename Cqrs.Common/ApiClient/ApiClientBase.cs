using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cqrs.Common.Api;
using Cqrs.Common.Exceptions;
using Cqrs.Common.Logging;
using Cqrs.Common.Utilities;

namespace Cqrs.Common.ApiClient
{
    public abstract class ApiClientBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILoggingService<ApiClientBase> _logger;

        protected ApiClientBase(HttpClient httpClient, ILoggingService<ApiClientBase> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected ILoggingService<ApiClientBase> Logger
        {
            get { return _logger; }
        }

        protected DependencyException GetDependencyServiceException<TResult>(Envelope<TResult> result)
        {
            if (!string.IsNullOrEmpty(result.ErrorCode) || !string.IsNullOrEmpty(result.ErrorMessage))
            {
                return new DependencyException(result.Url,
                    result.ErrorCode,
                    result.ErrorMessage,
                    result.Url);
            }

            return null;
        }

        protected void EnsureDependencyServiceSuccess<TResult>(Envelope<TResult> result)
        {
            var exception = GetDependencyServiceException(result);
            if (exception != null)
            {
                throw exception;
            }
        }

        protected async Task<Envelope<TResult>> GetAsync<TResult>(string url)
        {
            _logger.LogDebug($"ApiClientBase: Get {url}");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            return await SendAsync<TResult>(request);
        }

        protected async Task<Envelope<TResult>> PostAsync<TResult>(string url, object data)
        {
            _logger.LogDebug($"ApiClientBase: Post {url} {ApiHelper.GetLogString(data)}");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };

            return await SendAsync<TResult>(request);
        }

        protected async Task<Envelope<TResult>> PutAsync<TResult>(string url, object data)
        {
            _logger.LogDebug($"ApiClientBase: Put {url} {ApiHelper.GetLogString(data)}");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };

            return await SendAsync<TResult>(request);
        }

        protected async Task<Envelope<TResult>> DeleteAsync<TResult>(string url)
        {
            _logger.LogDebug($"ApiClientBase: Delete {url}");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Delete
            };

            return await SendAsync<TResult>(request);
        }

        protected async Task<Envelope<TResult>> SendAsync<TResult>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogCritical(null, "ApiClientBase: StatusCode {0}; Message {1}", response.StatusCode, await response.Content.ReadAsStringAsync());

                var ex = new HandledException(request.RequestUri.ToString(), $"{(int)response.StatusCode}", response.StatusCode.ToString());
                return new Envelope<TResult>(ex);
            }

            var draftResponse = await response.Content.ReadAsStringAsync();

            _logger.LogDebug($"ApiClientBase: Returned {draftResponse}");

            return JsonConvert.DeserializeObject<Envelope<TResult>>(draftResponse);
        }
    }
}
