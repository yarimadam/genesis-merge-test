namespace CoreType.Types
{
    public class FileContent
    {
        ///Should not be byte[] since there might be performance issues in the Service layer
        public string File { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
    }
}