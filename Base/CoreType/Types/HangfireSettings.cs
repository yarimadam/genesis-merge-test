using CoreType.Attributes;

namespace CoreType.Types
{
    public class HangfireSettings
    {
        public HangfireMailSettings Mail { get; set; }
    }

    public class HangfireMailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public bool UseSsl { get; set; }

        [HashedLogging]
        public string Password { get; set; }
    }
}