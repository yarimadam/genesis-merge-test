using Confluent.Kafka;

namespace CoreData.Infrastructure.Producers
{
    public class ProducerBase
    {
        // Create the producer configuration
        public static readonly ProducerConfig producerConfig = new ProducerConfig
        {
            BootstrapServers = ConfigurationManager.KafkaSettings.Url,
            //Debug = "topic",
            BatchNumMessages = 1,
            SocketNagleDisable = true
            //QueueBufferingMaxMessages =0 ,
        };
    }
}