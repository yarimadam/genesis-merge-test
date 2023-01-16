using System.Threading.Tasks;
using CoreData.Infrastructure.Consumers;
using CoreType.Models;
using CoreType.Types;

namespace CoreData.Infrastructure.Producers
{
    public class CommSMSProducer : Producer<Communication>
    {
        public static Task Produce(Communication data)
        {
            if (ConfigurationManager.KafkaSettings?.Enabled != true)
                return Task.Run(() => new CommSMSConsumer().HandleOnMessage(data));

            return Produce(ConfigurationManager.KafkaSettings.Topics[Topics.CommSMS].TopicName, data);
        }
    }
}