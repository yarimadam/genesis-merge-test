using System.Threading.Tasks;
using CoreData.Infrastructure.Consumers;
using CoreType.Models;
using CoreType.Types;

namespace CoreData.Infrastructure.Producers
{
    public class CommMailProducer : Producer<Communication>
    {
        public static Task Produce(Communication data)
        {
            if (ConfigurationManager.KafkaSettings?.Enabled != true)
                return Task.Run(() => new CommMailConsumer().HandleOnMessage(data));

            return Produce(ConfigurationManager.KafkaSettings.Topics[Topics.CommMail].TopicName, data);
        }
    }
}