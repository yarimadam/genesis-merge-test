using System;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Operations;
using Microsoft.EntityFrameworkCore;

namespace CoreData.Infrastructure
{
    public class UnitOfWork<TContext> : UnitOfWork where TContext : DbContext, new()
    {
        public UnitOfWork() : base(Helper.GetDbContext<TContext>())
        {
        }
    }

    public class UnitOfWork
    {
        public readonly DbContext Context;
        private readonly Type _contextType;

        public UnitOfWork()
        {
            Context = Helper.GetGenesisContext();
            _contextType = Context.GetType();
        }

        public UnitOfWork(DbContext context)
        {
            Context = context;
            _contextType = Context.GetType();
        }

        public UnitOfWorkTransactionScope BeginTransaction(ErrorBehaviour errorBehaviour = ErrorBehaviour.Rollback) =>
            new UnitOfWorkTransactionScope(Context).BeginTransaction(errorBehaviour);

        public async Task<UnitOfWorkTransactionScope> BeginTransactionAsync(ErrorBehaviour errorBehaviour = ErrorBehaviour.Rollback) =>
            await new UnitOfWorkTransactionScope(Context).BeginTransactionAsync(errorBehaviour);

        public UnitOfWork CreateInstance()
        {
            return new UnitOfWork(Helper.GetDbContext(_contextType));
        }

        public TRepository Get<TRepository>()
            where TRepository : IGenericRepository
            => TestHelpers.CreateRepository<TRepository>(Context);

        // public TRepository Get<TRepository, TEntity>()
        //     where TRepository : GenericRepository<TEntity>
        //     where TEntity : class, new()
        //     => TestHelpers.CreateRepository<TRepository>(Context);
        //
        // public IGenericRepository GetByEntity(Type type) => (IGenericRepository) typeof(UnitOfWork)
        //     .GetMethods()
        //     .FirstOrDefault(x => x.Name == nameof(Get) && x.IsGenericMethod)?
        //     .MakeGenericMethod(type)
        //     .Invoke(this, null);

        public void DetachAllEntities() => Context.DetachAllEntities();
    }
}