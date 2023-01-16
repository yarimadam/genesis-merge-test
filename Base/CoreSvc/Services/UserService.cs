using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CoreSvc.Services
{
    public class UserService : GenericService<CoreUsers, int, CoreUserValidator, UserRepository>
    {
        public UserService(DbContext context = null) : base(context)
        {
        }
    }
}