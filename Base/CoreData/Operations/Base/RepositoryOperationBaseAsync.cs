using System;
using CoreData.Infrastructure;
using CoreType.Types;

namespace CoreData.Operations
{
    public abstract class RepositoryOperationBaseAsync<TFunc, TResponse, TRepository> : OperationBaseAsync<TFunc, ResponseWrapper<TResponse>>
        where TRepository : GenericRepositoryBase
        where TFunc : Delegate
    {
        protected TRepository Repository { get; }

        protected RepositoryOperationBaseAsync(TRepository repository) : base(new UnitOfWork(repository.Context))
        {
            Repository = repository;
        }
    }

    public abstract class RepositoryOperationBaseAsync<TResponse, TRepository> : OperationBaseAsync<ResponseWrapper<TResponse>>
        where TRepository : GenericRepositoryBase
    {
        protected TRepository Repository { get; }

        protected RepositoryOperationBaseAsync(TRepository repository) : base(new UnitOfWork(repository.Context))
        {
            Repository = repository;
        }
    }
}