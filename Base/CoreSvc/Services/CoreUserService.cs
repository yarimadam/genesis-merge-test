using System;
using CoreSvc.Services;
using CoreType.DBModels;
using CoreData.Repositories;
using CoreData.Validators;

namespace CoreSvc.Services
{
    public class CoreUserService : GenericService<CoreUser, int, CoreUserValidator, CoreUserRepository>
    {
    }
}