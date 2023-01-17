using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class NotificationSetting
    {
        public NotificationSetting()
        {
            Notifications = new HashSet<Notification>();
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
        public int TenantId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
