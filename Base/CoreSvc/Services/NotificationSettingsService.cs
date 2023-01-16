using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreSvc.Services
{
    public class NotificationSettingsService : GenericService<NotificationSettings, int, NotificationSettingsValidator, NotificationSettingsRepository>
    {
    }
}