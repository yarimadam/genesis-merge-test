using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreSvc.Services
{
    public class NotificationService : GenericService<Notification, int, NotificationValidator, NotificationRepository>
    {
    }
}