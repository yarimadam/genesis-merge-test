using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using CoreData.Common;
using CoreData.Repositories;
using CoreType.DBModels;
using CoreType.Types;
using Serilog;

namespace CoreData.Infrastructure.Consumers
{
    public class LogConsumer : ConsumerBase
    {
        private readonly TransactionLogsRepository _transactionLogsRepository = new TransactionLogsRepository();

        public LogConsumer() : base(ConfigurationManager.KafkaSettings.Topics[Topics.TransactionLog].GroupId)
        {
        }

        public async Task Listen()
        {
            var settings = ConfigurationManager.KafkaSettings.Topics[Topics.TransactionLog];

            if (!IsEnabled)
            {
                Log.Debug("Consumer initiating skipped for {topic} due to Kafka is disabled via configuration.", settings.TopicName);
                return;
            }

            Log.Debug("Consumer initiating for {topic}", settings.TopicName);

            using (var consumer = new ConsumerBuilder<Ignore, TransactionLogs>(consumerConfig).SetValueDeserializer(new ObjectDeserializer<TransactionLogs>()).Build())
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

        public bool HandleOnMessage(TransactionLogs data)
        {
            try
            {
                using (SerilogExtensions.SuppressLogging())
                {
                    Task.WaitAll(_transactionLogsRepository.SaveAsync(data));
                }
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