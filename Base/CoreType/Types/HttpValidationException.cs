namespace CoreType.Types
{
    public class HttpValidationException : HttpResponseException
    {
        public HttpValidationException(string propertyName, string code, string message, string stackTrace = null)
            : base(code, message, stackTrace)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }
    }
}