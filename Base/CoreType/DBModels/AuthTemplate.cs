using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class AuthTemplate
    {
        public AuthTemplate()
        {
            AuthTemplateDetails = new HashSet<AuthTemplateDetail>();
        }

        public int AuthTemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDefaultPage { get; set; }
        public int TemplateType { get; set; }
        public int Status { get; set; }
        public bool IsDefault { get; set; }
        public int TenantId { get; set; }

        public virtual ICollection<AuthTemplateDetail> AuthTemplateDetails { get; set; }
    }
}
