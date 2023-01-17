using System.Collections.Generic;

namespace CoreType.DBModels
{
    public partial class AuthResources
    {
        public AuthResources()
        {
            AuthActions = new HashSet<AuthActions>();
        }

        public int ResourceId { get; set; }
        public string ParentResourceCode { get; set; }
        public string ResourceCode { get; set; }
        public string ResourceName { get; set; }
        public int ResourceType { get; set; }
        public int OrderIndex { get; set; }
        public int Status { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string TableName { get; set; }

        public virtual ICollection<AuthActions> AuthActions { get; set; }
    }
}