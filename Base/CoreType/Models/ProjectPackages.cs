using System.Collections.Generic;

namespace CoreType.Models
{
    public class ProjectPackages
    {
        public string ProjectName { get; set; }
        public string TargetFramework { get; set; }
        public List<PackageInformation> PackageInformations { get; set; } = new List<PackageInformation>();
    }
}