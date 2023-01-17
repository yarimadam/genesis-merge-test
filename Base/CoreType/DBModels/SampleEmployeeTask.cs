using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class SampleEmployeeTask
    {
        public int EmployeeTaskId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeTaskName { get; set; }
        public string EmployeeTaskDescription { get; set; }
        public string TaskTags { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
    }
}
