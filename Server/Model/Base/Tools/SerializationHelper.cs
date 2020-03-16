using System;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

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

            foreach (var type in AssemblyManagement.AllType.Where(d =>
                !d.IsInterface && typeof(IObject).IsAssignableFrom(d)))
            {
                BsonClassMap.LookupClassMap(type);
            }
        }

        public static string Serialize<T>(this T t)
        {
            return t.ToJson(JsonWriterSettings);
        }
        
        public static byte[] SerializeToByte<T>(this T t)
        {
            return t.ToBson();
        }

        public static object Deserialize(this string json, Type type)
        {
            return BsonSerializer.Deserialize(json, type);
        }

        public static T Deserialize<T>(this string json)
        {
            return BsonSerializer.Deserialize<T>(json);
        }

        public static T Deserialize<T>(this byte[] bytes)
        {
            return BsonSerializer.Deserialize<T>(bytes);
        }
        
        public static T Deserialize<T>(this Stream stream)
        {
            return BsonSerializer.Deserialize<T>(stream);
        }
    }
}