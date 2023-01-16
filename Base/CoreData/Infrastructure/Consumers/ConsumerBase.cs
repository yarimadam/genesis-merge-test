using Confluent.Kafka;

namespace CoreData.Infrastructure.Consumers
{
    public class ConsumerBase
    {
        public readonly ConsumerConfig consumerConfig;
        protected static bool IsEnabled => ConfigurationManager.KafkaSettings?.Enabled == true;

        public ConsumerBase(string GROUP_ID)
        {
            consumerConfig = new ConsumerConfig
            {
                GroupId = GROUP_ID,
                EnableAutoCommit = ConfigurationManager.KafkaSettings.AutoCommit,
                BootstrapServers = ConfigurationManager.KafkaSettings.Url,
                //Debug = "topic",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SocketNagleDisable = true
            };
        }
    }
}