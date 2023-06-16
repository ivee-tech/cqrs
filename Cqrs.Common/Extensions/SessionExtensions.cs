using Microsoft.AspNetCore.Http;

namespace Cqrs.Common.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, value.ToJsonString());
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value.ToObject<T>();
        }
    }
}
