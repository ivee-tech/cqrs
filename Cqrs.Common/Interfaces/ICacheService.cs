using System;

namespace Cqrs.Common.Interfaces
{
    public interface ICacheService
    {
        T GetValue<T>(string key);

        T GetOrAddValue<T>(string key, Func<T> factory, int expirationInSeconds = 0);

        void SetValue<T>(string key, T value, int expirationInSeconds = 0);
    }
}
