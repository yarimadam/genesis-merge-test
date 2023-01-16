using System.Collections.Generic;

namespace CoreType.DBModels
{
    public partial class AuthTemplate
    {
        public AuthTemplate()
        {
            AuthTemplateDetail = new HashSet<AuthTemplateDetail>();
        }

        public int AuthTemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDefaultPage { get; set; }
        public int TemplateType { get; set; }
        public int Status { get; set; }
        public bool IsDefault { get; set; }

        public virtual ICollection<AuthTemplateDetail> AuthTemplateDetail { get; set; }
    }
}