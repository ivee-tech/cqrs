using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http.Headers;
using Cqrs.Common.Logging;

namespace Cqrs.Common.Utilities
{
    public static class ApiHelper
    {
        public static bool IsDevelopment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development;
        public static bool IsProduction => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Production;

        public static string GetRequestHeaderValue(IHeaderDictionary headers, string name)
        {
            if (string.IsNullOrEmpty(name) || headers == null)
            {
                return null;
            }

            headers.TryGetValue(name, out StringValues values);
            return values.FirstOrDefault();
        }

        public static void AddRequestHeader(HttpRequestHeaders headers, string name, string value)
        {
            if (headers.Contains(name))
            {
                headers.Remove(name);
            }
            headers.Add(name, value);
        }

        public static string GetLogString(object value)
        {
            try
            {
                return JsonConvert.SerializeObject(value, Formatting.Indented, new LoggingJsonConverter());
            }
            catch
            {
            }
            return "(object cannot be serialized)";
        }
    }
}
