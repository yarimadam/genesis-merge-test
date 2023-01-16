using System;

namespace CoreType.Types
{
    public class JobStatus
    {
        public string JobId { get; set; }
        public string JobName { get; set; }
        public bool Status { get; set; }
        public DateTime? LastExecution { get; set; }
        public DateTime? NextExecution { get; set; }
    }
}