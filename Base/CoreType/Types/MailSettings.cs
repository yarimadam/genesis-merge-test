namespace CoreType.Types
{
    public class MailSettings
    {
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SenderAccount { get; set; }
        public string SenderPass { get; set; }
        public string SenderName { get; set; }
        public bool UseSSL { get; set; }
    }
}