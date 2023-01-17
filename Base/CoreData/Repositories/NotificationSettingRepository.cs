using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class NotificationSettingRepository : GenericGenesisRepository<NotificationSetting, int, NotificationSettingValidator>
    {
        public NotificationSettingRepository()
        {
        }

        public NotificationSettingRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}