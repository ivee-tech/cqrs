using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Cqrs.Common.Logging
{
    public class LoggingJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                JObject o = (JObject)t;

                foreach (var prop in o.Properties())
                {
                    if (prop.Value.Type == JTokenType.String && !string.IsNullOrEmpty(prop.Value?.ToString()) && prop.Value.ToString().Length > 500)
                    {
                        prop.Value = $"{prop.Value.ToString().Substring(0, 50)}...(size {prop.Value.ToString().Length})";
                    }
                }

                o.WriteTo(writer);
            }
        }
    }
}
