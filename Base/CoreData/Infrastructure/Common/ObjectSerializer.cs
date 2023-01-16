using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;
using Serilog;

namespace CoreData.Infrastructure
{
    public class ObjectSerializer<T> : ISerializer<T>
    {
        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            return config;
        }

        public byte[] Serialize(T data, SerializationContext context)
        {
            if (data == null)
                return null;

            byte[] bytes;
            try
            {
                bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "ObjectSerializer.Serialize");
                throw;
            }

            return bytes;
        }

        public void Dispose()
        {
        }
    }
}