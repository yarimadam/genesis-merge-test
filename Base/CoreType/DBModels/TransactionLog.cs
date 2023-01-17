using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class TransactionLog
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string ServiceUrl { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string CurrentClaims { get; set; }
        public int LogType { get; set; }
        public DateTime LogDateBegin { get; set; }
        public DateTime? LogDateEnd { get; set; }
        public int Status { get; set; }
        public int? StatusCode { get; set; }
        public int TenantId { get; set; }
    }
}
