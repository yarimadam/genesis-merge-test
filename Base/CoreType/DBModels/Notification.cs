using System;

namespace CoreType.DBModels
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int NotificationSettingsId { get; set; }
        public int? UserId { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int Status { get; set; }

        public virtual NotificationSettings NotificationSettings { get; set; }
    }
}