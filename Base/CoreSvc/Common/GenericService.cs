using CoreData.Common;
using CoreData.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CoreSvc.Services
{
    public abstract class GenericService : GenericServiceBase
    {
        protected GenericService(DbContext context = null) : base(context)
        {
        }
    }

    public abstract class GenericService<TRepository> : GenericServiceBase<TRepository>
        where TRepository : GenericRepositoryBase
    {
        protected GenericService(DbContext context = null) : base(context)
        {
        }
    }

    public abstract class GenericService<TEntity, TRepository> : GenericServiceBase<TEntity, TRepository>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase<TEntity>
    {
        protected GenericService(DbContext context = null) : base(context)
        {
        }
    }

    public abstract class GenericService<TEntity, TKey, TRepository> : GenericServiceBase<TEntity, TKey, TRepository>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase<TEntity, TKey>
    {
        protected GenericService(DbContext context = null) : base(context)
        {
        }
    }

    public abstract class GenericService<TEntity, TKey, TValidator, TRepository> : GenericServiceValidator<TEntity, TKey, TValidator, TRepository>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
        where TRepository : GenericRepositoryValidator<TEntity, TKey, TValidator>
    {
        protected GenericService(DbContext context = null) : base(context)
        {
        }
    }

    public abstract class GenericService<TEntity, TKey, TValidator, TContext, TRepository> : GenericServiceValidator<TEntity, TKey, TValidator, TRepository>
        where TContext : DbContext, new()
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
        where TRepository : GenericRepositoryValidator<TEntity, TKey, TValidator>
    {
        protected GenericService() : this(Helper.GetDbContext<TContext>())
        {
        }

        protected GenericService(TContext context = null) : base(context)
        {
        }
    }
}