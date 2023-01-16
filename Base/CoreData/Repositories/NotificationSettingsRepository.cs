using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreData.Repositories
{
    public class NotificationSettingsRepository : GenericGenesisRepository<NotificationSettings, int, NotificationSettingsValidator>
    {
        public NotificationSettingsRepository()
        {
        }

        public NotificationSettingsRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}