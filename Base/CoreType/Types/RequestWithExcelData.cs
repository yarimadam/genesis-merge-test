using System.Collections.Generic;

namespace CoreType.Types
{
    public class RequestWithExcelData<T>
    {
        public List<T> Data { get; set; }
        public object Type { get; set; }
        public int NotificationId { get; set; }
    }
}