using CoreData.Common;
using CoreType.Types;
using Microsoft.EntityFrameworkCore;

namespace CoreData.Infrastructure
{
    public abstract class GenericRepositoryContext<TContext> : GenericRepositoryBase, IGenericRepositoryContext<TContext>
        where TContext : DbContext, new()
    {
        public new TContext Context => (TContext) base.Context;

        protected GenericRepositoryContext() : base(Helper.GetDbContext<TContext>())
        {
        }

        protected GenericRepositoryContext(TContext context) : base(context)
        {
        }

        protected GenericRepositoryContext(SessionContext session) : base(Helper.GetDbContext<TContext>(session), session)
        {
        }

        protected GenericRepositoryContext(TContext context, SessionContext session) : base(context, session)
        {
        }
    }

    public abstract class GenericRepositoryContext<TEntity, TContext> : GenericRepositoryBase<TEntity>, IGenericRepositoryContext<TEntity, TContext>
        where TContext : DbContext, new()
        where TEntity : class, new()
    {
        public new TContext Context => (TContext) base.Context;

        protected GenericRepositoryContext() : base(Helper.GetDbContext<TContext>())
        {
        }

        protected GenericRepositoryContext(TContext context) : base(context)
        {
        }

        protected GenericRepositoryContext(SessionContext session) : base(Helper.GetDbContext<TContext>(session), session)
        {
        }

        protected GenericRepositoryContext(TContext context, SessionContext session) : base(context, session)
        {
        }
    }

    public abstract class GenericRepositoryContext<TEntity, TKey, TContext> : GenericRepositoryBase<TEntity, TKey>, IGenericRepositoryContext<TEntity, TKey, TContext>
        where TContext : DbContext, new()
        where TEntity : class, new()
    {
        public new TContext Context => (TContext) base.Context;

        protected GenericRepositoryContext() : base(Helper.GetDbContext<TContext>())
        {
        }

        protected GenericRepositoryContext(TContext context) : base(context)
        {
        }

        protected GenericRepositoryContext(SessionContext session) : base(Helper.GetDbContext<TContext>(session), session)
        {
        }

        protected GenericRepositoryContext(TContext context, SessionContext session) : base(context, session)
        {
        }
    }
}