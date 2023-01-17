using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class CommunicationDefinition
    {
        public int CommDefinitionId { get; set; }
        public string CommDefinitionName { get; set; }
        public short CommDefinitionType { get; set; }
        public string EmailSenderName { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }
        public string EmailSmtpServer { get; set; }
        public string EmailSenderAccount { get; set; }
        public short? EmailSecurityType { get; set; }
        public string EmailPort { get; set; }
        public string SmsCompanyName { get; set; }
        public string SmsProviderCode { get; set; }
        public string SmsCustomerCode { get; set; }
        public string SmsSenderNumber { get; set; }
        public string SmsEndpointUrl { get; set; }
        public string SmsAuthData { get; set; }
        public string SmsFormData { get; set; }
        public string SmsExpectedStatusCode { get; set; }
        public string SmsExpectedResponse { get; set; }
        public string SmsUsername { get; set; }
        public string SmsPassword { get; set; }
        public int? CompanyId { get; set; }
        public string Timezone { get; set; }
        public short Status { get; set; }
        public int TenantId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
