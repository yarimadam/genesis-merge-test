using System.Threading.Tasks;
using CoreData.Infrastructure.Consumers;
using CoreType.Types;

namespace CoreData.Infrastructure.Producers
{
    public class MailProducer : Producer<MailMessage>
    {
        public static Task Produce(MailMessage data)
        {
            if (ConfigurationManager.KafkaSettings?.Enabled != true)
                return Task.Run(() => new MailConsumer().HandleOnMessage(data));

            return Produce(ConfigurationManager.KafkaSettings.Topics[Topics.Mail].TopicName, data);
        }
    }
}