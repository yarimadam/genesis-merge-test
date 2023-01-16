namespace CoreType.DBModels
{
    public partial class CommunicationTemplates
    {
        public int CommTemplateId { get; set; }
        public string CommTemplateName { get; set; }
        public int CommDefinitionId { get; set; }
        public string EmailRecipients { get; set; }
        public string EmailCcs { get; set; }
        public string EmailBccs { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public bool? EmailIsBodyHtml { get; set; }
        public short? EmailPriority { get; set; }
        public string EmailSenderName { get; set; }
        public string SmsRecipients { get; set; }
        public string SmsBody { get; set; }
        public int? CompanyId { get; set; }
        public string Timezone { get; set; }
        public string ServiceUrls { get; set; }
        public string RequestType { get; set; }
        public string ResponseType { get; set; }
        public string RequestConditions { get; set; }
        public short Status { get; set; }
    }
}