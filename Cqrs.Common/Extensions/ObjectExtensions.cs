using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace Cqrs.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJsonString(this object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value,
                   new Newtonsoft.Json.JsonSerializerSettings
                   {
                       ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                   });
        }

        public static T ToObject<T>(this string value)
        {
            return value == null ? default : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value,
                  new Newtonsoft.Json.JsonSerializerSettings
                  {
                      ConstructorHandling = Newtonsoft.Json.ConstructorHandling.AllowNonPublicDefaultConstructor
                  });
        }

        public static T ToObject<T>(this JsonElement element, bool useCamelCase = true)
        {
            var json = element.GetRawText();
            T result;
            if (useCamelCase)
                result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            else
                result = JsonSerializer.Deserialize<T>(json);
            return result;
        }

        public static T ToObject<T>(this JsonDocument document)
        {
            return document.RootElement.ToObject<T>();
        }

        public static object ToObject(this JsonElement element, Type type, bool useCamelCase = true)
        {
            var json = element.GetRawText();
            object result;
            if (useCamelCase)
                result = JsonSerializer.Deserialize(json, type, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            else
                result = JsonSerializer.Deserialize(json, type);
            return result;
        }

        public static object ToObject(this JsonDocument document, Type type, bool useCamelCase = true)
        {
            return document.RootElement.ToObject(type, useCamelCase);
        }

        public static object BsonDocumentToObject(this BsonDocument document, Type type)
        {
            if (document == null) return null;
            if (type == null) throw new ArgumentNullException("type");
            // var value = BsonTypeMapper.MapToDotNetValue(document, new BsonTypeMapperOptions() { MapBsonDocumentTo = type }); // System.NotSupportedException : A BsonDocument can't be mapped to a List<Object>.
            // var value = BsonTypeMapper.MapToDotNetValue(document, new BsonTypeMapperOptions() { MapBsonDocumentTo = type });
            // var json = JsonSerializer.Serialize(value, value.GetType(), new JsonSerializerOptions() { });
            // var json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            // var o = JsonSerializer.Deserialize(json, type);
            // return value;
            var o = BsonSerializer.Deserialize(document, type);
            return o;
        }

        public static IEnumerable<object> BsonDocumentsToObjects(this IEnumerable<BsonDocument> documents, Type type)
        {
            if (documents == null) return new List<BsonDocument>();
            if (type == null) throw new ArgumentNullException("type");
            var objects = new List<object>();
            foreach (var doc in documents)
            {
                objects.Add(doc.BsonDocumentToObject(type));
            }
            return objects;
        }

        public static BsonDocument ObjectToBsonDocument(this object o)
        {
            if (o == null) return null;
            var document = o.ToBsonDocument(o.GetType());
            return document;
        }

        public static IEnumerable<BsonDocument> ObjectsToBsonDocuments(this IEnumerable<object> objects)
        {
            if (objects == null) return new List<BsonDocument>();
            var documents = new List<BsonDocument>();
            foreach (var obj in objects)
            {
                documents.Add(obj.ObjectToBsonDocument());
            }
            return documents;
        }

        public static JObject BsonDocumentToJObject(this BsonDocument document)
        {
            if (document == null) return null;
            // var json = document.ToJson(new JsonWriterSettings() { OutputMode = JsonOutputMode.Strict });
            // var o = JObject.Parse(json);
            var obj = BsonTypeMapper.MapToDotNetValue(document);
            var o = JObject.FromObject(obj);
            return o;
        }

        public static IEnumerable<JObject> BsonDocumentsToJObjects(this IEnumerable<BsonDocument> documents)
        {
            if (documents == null) return new List<JObject>();
            var jObjects = new List<JObject>();
            foreach (var doc in documents)
            {
                jObjects.Add(doc.BsonDocumentToJObject());
            }
            return jObjects;
        }

        public static BsonDocument JObjectToBsonDocument(this JObject jObject)
        {
            if (jObject == null) return null;
            var json = jObject.ToString();
            var document = BsonDocument.Parse(json);
            return document;
        }
        public static IEnumerable<BsonDocument> JObjectsToBsonDocuments(this IEnumerable<JObject> jObjects)
        {
            if (jObjects == null) return new List<BsonDocument>();
            var documents = new List<BsonDocument>();
            foreach (var jObj in jObjects)
            {
                documents.Add(jObj.JObjectToBsonDocument());
            }
            return documents;
        }

        public static string ToBsonObjectId(this Guid value)
        {
            return value.ToString().Replace("-", "").Substring(0, 24);
        }

        public static Guid FromBsonObjectId(this string value)
        {
            if (string.IsNullOrEmpty(value)) return Guid.Empty;
            var msg = "The value must have 24 hexa characters.";
            if (value.Length != 24) throw new ArgumentException("value", msg);
            var newValue = $"{value.Substring(0, 8)}-{value.Substring(8, 4)}-{value.Substring(12, 4)}-{value.Substring(16, 4)}-{value.Substring(20)}".PadRight(36, '0');
            Guid guid;
            if (!Guid.TryParse(newValue, out guid)) throw new ArgumentException("value", msg);
            return guid;
        }

        public static object GetValue(this object o, string propertyName)
        {
            var propInfo = o.GetType().GetProperty(propertyName);
            return propInfo.GetValue(o);
        }

        public static void SetValue(this object o, string propertyName, object propertyValue)
        {
            var propInfo = o.GetType().GetProperty(propertyName);
            propInfo.SetValue(o, propertyValue);
        }

        public static ExpandoObject ToExpando(this object o)
        {
            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            foreach (var property in o.GetType().GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(o));
            }
            return expando;
        }

        public static object FromExpando(ExpandoObject expando, Type type)
        {
            var o = Activator.CreateInstance(type);
            var dictionary = (IDictionary<string, object>)expando;

            foreach (var property in o.GetType().GetProperties())
            {
                if (dictionary.ContainsKey(property.Name))
                {
                    property.SetValue(o, dictionary[property.Name]);
                }
            }
            return o;
        }


    }
}
