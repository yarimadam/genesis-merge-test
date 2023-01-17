using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class NotificationRepository : GenericGenesisRepository<Notification, int, NotificationValidator>
    {
        public NotificationRepository()
        {
        }

        public NotificationRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}