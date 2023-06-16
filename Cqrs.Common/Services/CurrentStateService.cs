using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using Cqrs.Common.Constants;
using Cqrs.Common.Extensions;
using Cqrs.Common.Interfaces;
using Cqrs.Common.Utilities;

namespace Cqrs.Common.Services
{
    public class CurrentStateService : ICurrentStateService
    {
        private string TransactionGuid = "TransactionGuid";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CurrentStateService> _logger;

        private ISession Session => _httpContextAccessor?.HttpContext?.Session;

        public string TransactionId
        {
            get
            {
                var value = GetString(TransactionGuid);

                if (string.IsNullOrEmpty(value))
                {
                    value = ApiHelper.GetRequestHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, HeaderName.TransactionId);

                    if (string.IsNullOrEmpty(value))
                    {
                        value = Guid.NewGuid().ToString();
                        _logger.LogDebug($"[{value}] Start new transaction");
                    }
                    else
                    {
                        _logger.LogDebug($"[{value}] Continue transaction from upper stream API");
                    }

                    SetString(TransactionGuid, value);
                }

                return value;
            }
        }

        public string User
        {
            get
            {
                return ApiHelper.GetRequestHeaderValue(_httpContextAccessor?.HttpContext?.Request?.Headers, HeaderName.User);
            }
        }

        public CurrentStateService(IHttpContextAccessor httpContextAccessor,
            ICacheService cacheService,
            ILogger<CurrentStateService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
            _logger = logger;
        }

        public void SetString(string name, string value)
        {
            try
            {
                var stringValue = value;
                if (string.IsNullOrEmpty(stringValue))
                {
                    stringValue = string.Empty;
                }

                if (Session != null)
                {
                    Session.SetString(name, stringValue);
                }
                else
                {
                    _cacheService.SetValue(name, stringValue);
                }
            }
            catch
            {
            }
        }

        public string GetString(string name)
        {
            try
            {
                if (Session != null)
                {
                    return Session.GetString(name);
                }
                else
                {
                    return _cacheService.GetValue<string>(name);
                }
            }
            catch
            {
            }
            return string.Empty;
        }

        public void SetObject(string name, object value)
        {
            try
            {
                if (Session != null)
                {
                    Session.SetObjectAsJson(name, value);
                }
                else
                {
                    _cacheService.SetValue(name, value);
                }
            }
            catch
            {
            }
        }

        public T GetObject<T>(string name)
        {
            try
            {
                if (Session != null)
                {
                    return Session.GetObjectFromJson<T>(name);
                }
                else
                {
                    return _cacheService.GetValue<T>(name);
                }
            }
            catch
            {
            }
            return default;
        }
    }
}
