using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;
using Serilog;

namespace CoreData.Infrastructure
{
    public class ObjectDeserializer<T> : IDeserializer<T>
    {
        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            return config;
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (data == null)
                return default;

            T obj;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data.ToArray()),
                    new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "ObjectDeserializer.Deserialize");
                throw;
            }

            return obj;
        }

        public void Dispose()
        {
        }
    }
}