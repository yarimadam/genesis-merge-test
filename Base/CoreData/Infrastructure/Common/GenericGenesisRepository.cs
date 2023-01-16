using CoreData.Common;
using CoreType.Types;
using FluentValidation;

namespace CoreData.Infrastructure
{
    public abstract class GenericGenesisRepository : GenericRepository
    {
        public new GenesisContextBase Context => (GenesisContextBase) base.Context;

        protected GenericGenesisRepository() : this(Helper.GetGenesisContext())
        {
        }

        protected GenericGenesisRepository(SessionContext session) : this(Helper.GetGenesisContext(session), session)
        {
        }

        protected GenericGenesisRepository(GenesisContextBase context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public abstract class GenericGenesisRepository<TEntity> : GenericRepository<TEntity>
        where TEntity : class, new()
    {
        public new GenesisContextBase Context => (GenesisContextBase) base.Context;

        protected GenericGenesisRepository() : this(Helper.GetGenesisContext())
        {
        }

        protected GenericGenesisRepository(SessionContext session) : this(Helper.GetGenesisContext(session), session)
        {
        }

        protected GenericGenesisRepository(GenesisContextBase context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public abstract class GenericGenesisRepository<TEntity, TKey> : GenericRepository<TEntity, TKey>
        where TEntity : class, new()
    {
        public new GenesisContextBase Context => (GenesisContextBase) base.Context;

        protected GenericGenesisRepository() : this(Helper.GetGenesisContext())
        {
        }

        protected GenericGenesisRepository(SessionContext session) : this(Helper.GetGenesisContext(session), session)
        {
        }

        protected GenericGenesisRepository(GenesisContextBase context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public abstract class GenericGenesisRepository<TEntity, TKey, TValidator> : GenericRepository<TEntity, TKey, TValidator>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        public new GenesisContextBase Context => (GenesisContextBase) base.Context;

        protected GenericGenesisRepository() : this(Helper.GetGenesisContext())
        {
        }

        protected GenericGenesisRepository(SessionContext session) : this(Helper.GetGenesisContext(session), session)
        {
        }

        protected GenericGenesisRepository(GenesisContextBase context, SessionContext session = null) : base(context, session)
        {
        }
    }

    public abstract class GenericGenesisRepository<TEntity, TKey, TValidator, TContext> : GenericRepository<TEntity, TKey, TValidator>
        where TContext : GenesisContextBase, new()
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        public new TContext Context => (TContext) base.Context;

        protected GenericGenesisRepository() : this(Helper.GetDbContext<TContext>())
        {
        }

        protected GenericGenesisRepository(SessionContext session) : this(Helper.GetDbContext<TContext>(session), session)
        {
        }

        protected GenericGenesisRepository(TContext context, SessionContext session = null) : base(context, session)
        {
        }
    }
}