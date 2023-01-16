using System;
using Microservice.TypeLib.DBModels;
using Microservice.DataLib.Common;
using Microservice.DataLib.DBContexts;
using Microservice.DataLib.Validators;

namespace Microservice.DataLib.Repositories
{
    public class UserRepository : GenericRepository<User, Guid?, UserValidator>
    {
        public UserRepository()
        {
        }

        public UserRepository(user_appContext context) : base(context)
        {
        }
    }
}