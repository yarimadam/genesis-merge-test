namespace CoreType.Types
{
    public class MailMessage
    {
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
    }
}