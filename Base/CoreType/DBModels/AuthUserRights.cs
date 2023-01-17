using Newtonsoft.Json;

namespace CoreType.DBModels
{
    public partial class AuthUserRights
    {
        public int RightId { get; set; }
        public int ActionId { get; set; }
        public int UserId { get; set; }
        public int RecordType { get; set; }
        public int? GroupActionId { get; set; }
        public int Status { get; set; }

        [JsonIgnore]
        public virtual AuthActions Action { get; set; }

        [JsonIgnore]
        public virtual CoreUsers User { get; set; }
    }
}