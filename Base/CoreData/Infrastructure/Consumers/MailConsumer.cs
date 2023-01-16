using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using CoreData.Common;
using CoreType.Types;
using Serilog;

namespace CoreData.Infrastructure.Consumers
{
    public class MailConsumer : ConsumerBase
    {
        public MailConsumer() : base(ConfigurationManager.KafkaSettings.Topics[Topics.Mail].GroupId)
        {
        }

        public async Task Listen()
        {
            var settings = ConfigurationManager.KafkaSettings.Topics[Topics.Mail];

            if (!IsEnabled)
            {
                Log.Debug("Consumer initiating skipped for {topic} due to Kafka is disabled via configuration.", settings.TopicName);
                return;
            }

            Log.Debug("Consumer initiating for {topic}", settings.TopicName);

            using (var consumer = new ConsumerBuilder<Ignore, MailMessage>(consumerConfig).SetValueDeserializer(new ObjectDeserializer<MailMessage>()).Build())
            {
                Log.Debug("Consumer initiated");

                Log.Debug("Subscribing for {topic}", settings.TopicName);
                consumer.Subscribe(settings.TopicName);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume();

                            Log.Debug("Message received for '{topic}' at: '{topicPartitionOffset}'.", settings.TopicName, cr.TopicPartitionOffset);
                            if (HandleOnMessage(cr.Value))
                                if (ConfigurationManager.KafkaSettings.AutoCommit == false)
                                    consumer.Commit(cr);
                        }
                        catch (ConsumeException e)
                        {
                            Log.Fatal(e, "ConsumeException");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();
                }
            }
        }

        public bool HandleOnMessage(MailMessage data)
        {
            try
            {
                CommunicationManager.Mail.SendViaSettings(data);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "HandleOnMessage", data);
                return false;
            }

            return true;
        }
    }
}