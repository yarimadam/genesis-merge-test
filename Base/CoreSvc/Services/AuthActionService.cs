using System;
using CoreSvc.Services;
using CoreType.DBModels;
using CoreData.Repositories;
using CoreData.Validators;

namespace CoreSvc.Services
{
    public class AuthActionService : GenericService<AuthAction, int, AuthActionValidator, AuthActionRepository>
    {
    }
}