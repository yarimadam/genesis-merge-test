using System.Collections.Generic;

namespace CoreType.Types
{
    public enum Topics
    {
        Mail,
        TransactionLog,
        CommMail,
        CommSMS
    }

    public class KafkaSettings
    {
        public bool Enabled { get; set; }
        public string Url { get; set; }
        public bool AutoCommit { get; set; }
        public Dictionary<Topics, KafkaTopic> Topics { get; set; } = new Dictionary<Topics, KafkaTopic>();
    }

    public class KafkaTopic
    {
        public string TopicName { get; set; }
        public string GroupId { get; set; }
        public int Interval { get; set; }
    }
}