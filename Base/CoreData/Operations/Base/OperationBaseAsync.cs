using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Infrastructure;

namespace CoreData.Operations
{
    public abstract class OperationBaseAsync<TResponse> : OperationBase, IOperationBaseAsync<TResponse>
        where TResponse : new()
    {
        protected OperationBaseAsync(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected virtual async Task<bool> CanExecuteAsync()
        {
            return await Task.FromResult(true);
        }

        protected virtual async Task PreExecuteAsync()
        {
            var canExecute = await CanExecuteAsync();
            if (!canExecute)
                throw new GenesisException("Operation execution is not permitted !");
        }

        protected abstract Task<TResponse> OnExecuteAsync(TResponse response);

        public TResponse Execute()
        {
            return ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<TResponse> ExecuteAsync()
        {
            await PreExecuteAsync();

            var response = new TResponse();
            response = await OnExecuteAsync(response);

            await PostExecuteAsync(response);

            return response;
        }

        protected virtual async Task PostExecuteAsync(TResponse response)
        {
            await Task.CompletedTask;
        }
    }

    public abstract class OperationBaseAsync<TRequest, TResponse> : OperationBase, IOperationBaseAsync<TRequest, TResponse>
        where TResponse : new()
    {
        protected OperationBaseAsync(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected virtual async Task<bool> CanExecuteAsync(TRequest request)
        {
            return await Task.FromResult(true);
        }

        protected virtual async Task PreExecuteAsync(TRequest request)
        {
            var canExecute = await CanExecuteAsync(request);
            if (!canExecute)
                throw new GenesisException("Operation execution is not permitted !");
        }

        protected abstract Task<TResponse> OnExecuteAsync(TRequest request, TResponse response);

        public TResponse Execute(TRequest request = default)
        {
            return ExecuteAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<TResponse> ExecuteAsync(TRequest request = default)
        {
            await PreExecuteAsync(request);

            var response = new TResponse();
            response = await OnExecuteAsync(request, response);

            await PostExecuteAsync(request, response);

            return response;
        }

        protected virtual async Task PostExecuteAsync(TRequest request, TResponse response)
        {
            await Task.CompletedTask;
        }
    }
}