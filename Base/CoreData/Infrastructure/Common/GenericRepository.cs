using CoreData.Common;
using CoreType.Types;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CoreData.Infrastructure
{
    public abstract class GenericRepository : GenericRepositoryBase
    {
        protected GenericRepository(DbContext context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public abstract class GenericRepository<TEntity> : GenericRepositoryBase<TEntity>
        where TEntity : class, new()
    {
        protected GenericRepository(DbContext context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public abstract class GenericRepository<TEntity, TKey> : GenericRepositoryBase<TEntity, TKey>
        where TEntity : class, new()
    {
        protected GenericRepository(DbContext context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public abstract class GenericRepository<TEntity, TKey, TValidator> : GenericRepositoryValidator<TEntity, TKey, TValidator>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        protected GenericRepository(DbContext context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public abstract class GenericRepository<TEntity, TKey, TValidator, TContext> : GenericRepositoryValidator<TEntity, TKey, TValidator>
        where TContext : DbContext, new()
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        public new TContext Context => (TContext) base.Context;

        protected GenericRepository() : this(Helper.GetDbContext<TContext>())
        {
        }

        protected GenericRepository(SessionContext session) : base(Helper.GetDbContext<TContext>(session), session)
        {
        }

        protected GenericRepository(TContext context, SessionContext session = null) : base(context, session)
        {
        }
    }
}