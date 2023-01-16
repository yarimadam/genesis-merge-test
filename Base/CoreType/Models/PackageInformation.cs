namespace CoreType.Models
{
    public class PackageInformation
    {
        public string PackageName { get; set; }
        public string RequestedVersion { get; set; }
        public string ResolvedVersion { get; set; }
        public bool IsAutoReferenced { get; set; }
    }
}