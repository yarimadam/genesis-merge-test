using System.Threading.Tasks;
using CoreData.Infrastructure.Consumers;
using CoreType.DBModels;
using CoreType.Types;

namespace CoreData.Infrastructure.Producers
{
    public class LogProducer : Producer<TransactionLogs>
    {
        public static Task Produce(TransactionLogs data)
        {
            if (ConfigurationManager.KafkaSettings?.Enabled != true)
                return Task.Run(() => new LogConsumer().HandleOnMessage(data));

            return Produce(ConfigurationManager.KafkaSettings.Topics[Topics.TransactionLog].TopicName, data);
        }
    }
}