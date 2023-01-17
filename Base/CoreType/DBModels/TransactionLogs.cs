using System;
using CoreType.Types;

namespace CoreType.DBModels
{
    public partial class TransactionLogs
    {
        public TransactionLogs()
        {
            Request = new RequestLog();
            Response = new ResponseLog();
        }

        public int LogId { get; set; }
        public int UserId { get; set; }
        public string ServiceUrl { get; set; }
        public RequestLog Request { get; set; }
        public ResponseLog Response { get; set; }
        public object CurrentClaims { get; set; }
        public int LogType { get; set; }
        public DateTime LogDateBegin { get; set; }
        public DateTime? LogDateEnd { get; set; }
        public int Status { get; set; }
        public int? StatusCode { get; set; }
    }
}