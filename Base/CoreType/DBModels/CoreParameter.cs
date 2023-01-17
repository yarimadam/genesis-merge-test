using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class CoreParameter
    {
        public int ParameterId { get; set; }
        public string KeyCode { get; set; }
        public int ParentValue { get; set; }
        public string Value { get; set; }
        public int? OrderIndex { get; set; }
        public int? Status { get; set; }
        public string Description { get; set; }
        public string Translations { get; set; }
        public int TenantId { get; set; }
    }
}
