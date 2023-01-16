using System.Threading.Tasks;

namespace CoreData.Operations
{
    public interface IOperationBaseAsync<TResponse> : IOperationBase<TResponse>
        where TResponse : new()
    {
        Task<TResponse> ExecuteAsync();
    }

    public interface IOperationBaseAsync<TRequest, TResponse> : IOperationBase<TRequest, TResponse>
        where TResponse : new()
    {
        Task<TResponse> ExecuteAsync(TRequest request = default);
    }

    public interface IOperationBase<TResponse> : IOperationBase
    {
        TResponse Execute();
    }

    public interface IOperationBase<TRequest, TResponse> : IOperationBase
    {
        TResponse Execute(TRequest request = default);
    }

    public interface IOperationBase
    {
    }
}