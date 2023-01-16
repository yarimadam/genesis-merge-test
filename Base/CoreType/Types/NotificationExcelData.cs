using System.Collections.Generic;

namespace CoreType.Types
{
    public class NotificationExcelData<TEntity>
    {
        public string ServicePath { get; set; }
        public object ClientType { get; set; }
        public List<ResponseWrapper<TEntity>> UnsuccessfulRecords { get; set; } = new List<ResponseWrapper<TEntity>>();
        public double ElapsedTime { get; set; }
    }
}