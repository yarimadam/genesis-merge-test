using System;
using System.Collections.Generic;

#nullable disable

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
        public int TenantId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual NotificationSetting NotificationSettings { get; set; }
    }
}
