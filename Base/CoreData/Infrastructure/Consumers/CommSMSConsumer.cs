using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using CoreData.Common;
using CoreType.Models;
using CoreType.Types;
using Serilog;

namespace CoreData.Infrastructure.Consumers
{
    public class CommSMSConsumer : ConsumerBase
    {
        public CommSMSConsumer() : base(ConfigurationManager.KafkaSettings.Topics[Topics.CommSMS].GroupId)
        {
        }

        public async Task Listen()
        {
            var settings = ConfigurationManager.KafkaSettings.Topics[Topics.CommSMS];

            if (!IsEnabled)
            {
                Log.Debug("Consumer initiating skipped for {topic} due to Kafka is disabled via configuration.", settings.TopicName);
                return;
            }

            Log.Debug("Consumer initiating for {topic}", settings.TopicName);

            using (var consumer = new ConsumerBuilder<Ignore, Communication>(consumerConfig).SetValueDeserializer(new ObjectDeserializer<Communication>()).Build())
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

        public bool HandleOnMessage(Communication data)
        {
            try
            {
                CommunicationManager.SMS.Send(data);
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