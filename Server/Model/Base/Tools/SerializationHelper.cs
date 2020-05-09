using System;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Sining.Tools
{
    public static class SerializationHelper
    {
        private static readonly JsonWriterSettings JsonWriterSettings = new JsonWriterSettings()
            {OutputMode = JsonOutputMode.Strict};

        public static void Init()
        {
            // 自动注册IgnoreExtraElements

            var conventionPack = new ConventionPack {new IgnoreExtraElementsConvention(true)};

            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

            foreach (var type in AssemblyManagement.AllType.Values.SelectMany(allTypes => allTypes.Where(d =>
                !d.IsInterface && typeof(IObject).IsAssignableFrom(d))))
            {
                BsonClassMap.LookupClassMap(type);
            }
        }

        public static string ToJson<T>(this T t)
        {
            return JsonConvert.SerializeObject(t);
        }
        
        public static byte[] ToBytes<T>(this T t)
        {
            return t.ToBson();
        }

        public static object Deserialize(this string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static T Deserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T Deserialize<T>(this byte[] bytes)
        {
            return BsonSerializer.Deserialize<T>(bytes);
        }
        
        public static T Deserialize<T>(this Stream stream)
        {
            return BsonSerializer.Deserialize<T>(stream);
        }
        
        public static object Deserialize(this Stream stream,Type type)
        {
            return BsonSerializer.Deserialize(stream, type);
        }

        public static T Clone<T>(this T t)
        {
            return Deserialize<T>(ToBytes(t));
        }
    }
}