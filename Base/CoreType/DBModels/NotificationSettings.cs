using System.Collections.Generic;

namespace CoreType.DBModels
{
    public partial class NotificationSettings
    {
        public NotificationSettings()
        {
            Notification = new HashSet<Notification>();
        }

        public int NotificationSettingsId { get; set; }
        public int NotificationType { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public int? UserId { get; set; }
        public int? CompanyId { get; set; }
        public int? Gender { get; set; }
        public int? City { get; set; }
        public string AgeRange { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Notification> Notification { get; set; }
    }
}