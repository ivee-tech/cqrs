using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Cqrs.Common.Exceptions;
using Cqrs.Common.Utilities;

namespace Cqrs.Common.Api
{
    public class Envelope<TResult>
    {
        public string Url { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }

        public IEnumerable<TResult> Results { get; set; }

        [JsonIgnore]
        public TResult Result
        {
            get
            {
                if (Results != null)
                {
                    return Results.FirstOrDefault();
                }

                return default;
            }
        }

        // used for default serialization / deserialization
        public Envelope()
        {

        }

        public Envelope(string selfUrl)
            : this(selfUrl, null, null)
        {
        }

        public Envelope(string selfUrl, TResult result)
            : this(selfUrl, null, null, result)
        {
        }

        public Envelope(string selfUrl, IEnumerable<TResult> results)
            : this(selfUrl, null, null, results)
        {
        }

        public Envelope(string selfUrl, string code, string message, TResult result)
            : this(selfUrl, code, message)
        {
            if (result != null)
            {
                Results = new[] { result };
            }
        }

        public Envelope(string selfUrl, string code, string message, IEnumerable<TResult> results = null)
        {
            Url = selfUrl;

            ErrorCode = code;
            ErrorMessage = message;

            Results = results;
        }

        public Envelope(HandledException exception)
            : this(exception?.Url, exception?.Code, exception?.Message)
        {
            if (ApiHelper.IsDevelopment)
            {
                ErrorMessage = exception.Message;
            }
        }
    }

    public class PaginatedEnvelope<TResult> : Envelope<TResult>
    {
        public int? PageIndex { get; private set; }
        public int? TotalCount { get; private set; }
        public int? TotalPages { get; private set; }

        public PaginatedEnvelope(string selfUrl, IEnumerable<TResult> results, int? pageIndex, int? totalCount, int? totalPages)
            : this(selfUrl, null, null, results, pageIndex, totalCount, totalPages)
        {
        }

        public PaginatedEnvelope(string selfUrl, string code, string message, IEnumerable<TResult> results, int? pageIndex, int? totalCount, int? totalPages)
            : base(selfUrl, code, message, results)
        {
            PageIndex = pageIndex;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }
    }
}
