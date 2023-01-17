using FluentValidation;
using CoreType.Types;
using CoreData.Infrastructure;
using CoreData.DBContexts;

namespace CoreData.Common
{
    public class GenericRepository : GenericRepositoryContext<genesisContext_PostgreSQL>
    {
        protected GenericRepository()
        {
        }

        protected GenericRepository(SessionContext session) : base(session)
        {
        }

        protected GenericRepository(genesisContext_PostgreSQL context, SessionContext session = null) : base(context, session)
        {
        }
    }
    
    public class GenericRepository<TEntity> : GenericRepositoryContext<TEntity, genesisContext_PostgreSQL>
        where TEntity : class, new()
    {
        protected GenericRepository()
        {
        }

        protected GenericRepository(SessionContext session) : base(session)
        {
        }

        protected GenericRepository(genesisContext_PostgreSQL context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public class GenericRepository<TEntity, TKey> : GenericRepositoryContext<TEntity, TKey, genesisContext_PostgreSQL>
        where TEntity : class, new()
    {
        protected GenericRepository()
        {
        }

        protected GenericRepository(SessionContext session) : base(session)
        {
        }

        protected GenericRepository(genesisContext_PostgreSQL context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public class GenericRepository<TEntity, TKey, TValidator> : GenericRepository<TEntity, TKey, TValidator, genesisContext_PostgreSQL>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        protected GenericRepository()
        {
        }

        protected GenericRepository(SessionContext session) : base(session)
        {
        }

        protected GenericRepository(genesisContext_PostgreSQL context, SessionContext session = null) : base(context, session)
        {
        }
    }
}