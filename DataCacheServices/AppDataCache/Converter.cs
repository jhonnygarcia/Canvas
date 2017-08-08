using System;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DataCacheServices.AppDataCache
{
    internal enum SerializationFormat
    {
        Json,
        Binary
    }

    internal static class Converter
    {

        public static string Serialize(this object obj, SerializationFormat format = SerializationFormat.Json)
        {
            if (format == SerializationFormat.Json)
                return SerializeToJson(obj);
            if (format == SerializationFormat.Binary)
                return SerializeToBinary(obj);
            return "";
        }

        public static T Deserialize<T>(this string strObj, SerializationFormat format = SerializationFormat.Json)
        {
            if (format == SerializationFormat.Json)
                return DeserealizeFromJson<T>(strObj);
            if (format == SerializationFormat.Binary)
                return DeserealizeFromBinary<T>(strObj);
            return default(T);
        }

        private static string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static T DeserealizeFromJson<T>(string strObj)
        {
            if (string.IsNullOrEmpty(strObj)) strObj = "";
            return JsonConvert.DeserializeObject<T>(strObj);
        }

        private static string SerializeToBinary(object obj)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new NetDataContractSerializer();
                serializer.Serialize(stream, obj);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        private static T DeserealizeFromBinary<T>(string strObj)
        {
            if (string.IsNullOrEmpty(strObj)) return default(T);
            var b = Convert.FromBase64String(strObj);
            using (var stream = new MemoryStream(b))
            {
                var serializer = new NetDataContractSerializer();
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(stream);
            }
        }
    }
}
