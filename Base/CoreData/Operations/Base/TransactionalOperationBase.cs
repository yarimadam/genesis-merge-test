using System;
using CoreData.Common;
using CoreData.Infrastructure;

namespace CoreData.Operations
{
    public enum ErrorBehaviour
    {
        CommitAnyway,
        Rollback,
        DoNothing // Managed externally
    }

    public abstract class TransactionalOperation<TRequest> : TransactionalOperation<TRequest, TRequest>
        where TRequest : new()
    {
        protected TransactionalOperation(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    public abstract class TransactionalOperation<TRequest, TResponse> : OperationBase<TRequest, TResponse>
        where TResponse : new()
    {
        protected ErrorBehaviour Behaviour = ErrorBehaviour.Rollback;
        protected UnitOfWorkTransactionScope Transaction;

        protected TransactionalOperation(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected abstract override TResponse OnExecute(TRequest request, TResponse response);

        public override TResponse Execute(TRequest request = default)
        {
            return ExecuteTransactional(() => base.Execute(request));
        }

        protected TResponse ExecuteTransactional(Func<TResponse> executeFunc)
        {
            using (Transaction = UnitOfWork.BeginTransaction(ErrorBehaviour.DoNothing))
            {
                try
                {
                    var response = executeFunc();

                    Transaction.Commit();

                    return response;
                }
                catch (GenesisException e)
                {
                    if (Behaviour == ErrorBehaviour.CommitAnyway)
                        Transaction.Commit();
                    else if (Behaviour != ErrorBehaviour.DoNothing)
                        Transaction.Rollback();
                }
                catch (Exception e)
                {
                    if (Behaviour != ErrorBehaviour.DoNothing)
                        Transaction.Rollback();
                }
            }

            return default;
        }
    }
}