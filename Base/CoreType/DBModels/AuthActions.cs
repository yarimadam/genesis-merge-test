using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoreType.DBModels
{
    public partial class AuthActions
    {
        public AuthActions()
        {
            AuthUserRights = new HashSet<AuthUserRights>();
        }

        public int ActionId { get; set; }
        public int ResourceId { get; set; }
        public int ActionType { get; set; }
        public int OrderIndex { get; set; }
        public int Status { get; set; }

        [JsonIgnore]
        public virtual AuthResources Resource { get; set; }

        public virtual ICollection<AuthUserRights> AuthUserRights { get; set; }
    }
}