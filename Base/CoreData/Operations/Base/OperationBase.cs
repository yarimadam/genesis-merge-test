using CoreData.Common;
using CoreData.Infrastructure;

namespace CoreData.Operations
{
    public abstract class OperationBase
    {
        protected readonly UnitOfWork UnitOfWork;

        protected OperationBase(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }

    public abstract class OperationBase<TResponse> : OperationBase, IOperationBase<TResponse>
        where TResponse : new()
    {
        protected OperationBase(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected virtual bool CanExecute()
        {
            return true;
        }

        protected virtual void PreExecute()
        {
            if (!CanExecute())
                throw new GenesisException("Operation execution is not permitted !");
        }

        protected abstract TResponse OnExecute(TResponse response);

        public virtual TResponse Execute()
        {
            PreExecute();

            var response = new TResponse();
            response = OnExecute(response);

            PostExecute(response);

            return response;
        }

        protected virtual void PostExecute(TResponse response)
        {
        }
    }

    public abstract class OperationBase<TRequest, TResponse> : OperationBase, IOperationBase<TRequest, TResponse>
        where TResponse : new()
    {
        protected OperationBase(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected virtual bool CanExecute(TRequest request)
        {
            return true;
        }

        protected virtual void PreExecute(TRequest request)
        {
            if (!CanExecute(request))
                throw new GenesisException("Operation execution is not permitted !");
        }

        protected abstract TResponse OnExecute(TRequest request, TResponse response);

        public virtual TResponse Execute(TRequest request = default)
        {
            PreExecute(request);

            var response = new TResponse();
            response = OnExecute(request, response);

            PostExecute(request, response);

            return response;
        }

        protected virtual void PostExecute(TRequest request, TResponse response)
        {
        }
    }
}