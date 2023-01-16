using FluentValidation;
using CoreType.Types;
using CoreData.Infrastructure;
using Microservice.DataLib.DBContexts;

namespace Microservice.DataLib.Common
{
    public class GenericRepository : GenericRepositoryContext<user_appContext>
    {
        protected GenericRepository()
        {
        }

        protected GenericRepository(SessionContext session) : base(session)
        {
        }

        protected GenericRepository(user_appContext context, SessionContext session = null) : base(context, session)
        {
        }
    }
    
    public class GenericRepository<TEntity> : GenericRepositoryContext<TEntity, user_appContext>
        where TEntity : class, new()
    {
        protected GenericRepository()
        {
        }

        protected GenericRepository(SessionContext session) : base(session)
        {
        }

        protected GenericRepository(user_appContext context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public class GenericRepository<TEntity, TKey> : GenericRepositoryContext<TEntity, TKey, user_appContext>
        where TEntity : class, new()
    {
        protected GenericRepository()
        {
        }

        protected GenericRepository(SessionContext session) : base(session)
        {
        }

        protected GenericRepository(user_appContext context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public class GenericRepository<TEntity, TKey, TValidator> : GenericRepository<TEntity, TKey, TValidator, user_appContext>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        protected GenericRepository()
        {
        }

        protected GenericRepository(SessionContext session) : base(session)
        {
        }

        protected GenericRepository(user_appContext context, SessionContext session = null) : base(context, session)
        {
        }
    }
}