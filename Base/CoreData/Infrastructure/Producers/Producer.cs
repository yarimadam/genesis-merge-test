using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Serilog;

namespace CoreData.Infrastructure.Producers
{
    public class Producer<T> : ProducerBase
    {
        private static readonly ObjectSerializer<T> serializer = new ObjectSerializer<T>();

        public static async Task<DeliveryResult<Null, T>> Produce(string topic, T data)
        {
            try
            {
                Log.Debug("Producer initiating for {topic}", topic);

                using (var producer = new ProducerBuilder<Null, T>(producerConfig).SetValueSerializer(serializer).Build())
                {
                    Log.Debug("Producer initiated");

                    Log.Debug("Producing async");

                    try
                    {
                        return await producer.ProduceAsync(topic, new Message<Null, T> { Value = data });
                    }
                    catch (ProduceException<Null, string> ex)
                    {
                        Log.Fatal(ex, "Delivery failed: {reason}", ex.Error.Reason);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Producer failed for {topic}", topic);
            }

            return null;
        }

        public static T ProduceAndGetResult(string topic, T data)
        {
            return Produce(topic, data).ConfigureAwait(false).GetAwaiter().GetResult().Value;
        }
    }
}