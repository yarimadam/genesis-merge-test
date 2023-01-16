using System;
using CoreSvc.Services;
using Microservice.TypeLib.DBModels;
using Microservice.DataLib.Repositories;
using Microservice.DataLib.Validators;

namespace Microservice.API.Services
{
    public class UserService : GenericService<User, Guid?, UserValidator, UserRepository>
    {
    }
}