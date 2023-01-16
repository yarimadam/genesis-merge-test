using Microsoft.EntityFrameworkCore;

namespace CoreData.Infrastructure
{
    public interface IGenericRepositoryContext<TEntity, TKey, TContext> : IGenericRepositoryContext<TEntity, TContext>, IGenericRepository<TEntity, TKey>
        where TEntity : class, new()
        where TContext : DbContext, new()
    {
    }

    public interface IGenericRepositoryContext<TEntity, TContext> : IGenericRepositoryContext<TContext>, IGenericRepository<TEntity>
        where TEntity : class, new()
        where TContext : DbContext, new()
    {
    }

    public interface IGenericRepositoryContext<TContext> : IGenericRepositoryContext, IGenericRepository
        where TContext : DbContext, new()
    {
        new TContext Context { get; }
    }

    public interface IGenericRepositoryContext
    {
    }
}