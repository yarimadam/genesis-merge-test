namespace CoreType.Types
{
    public class RequestLog
    {
        public string ServiceUrlFull { get; set; }
        public object RequestBody { get; set; }
        public object OriginalRequestBody { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public string RemoteIP { get; set; }
    }

    public class ResponseLog
    {
        public object ResponseBody { get; set; }
        public double ProcessElapsedTime { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorMessageDetail { get; set; }
    }
}