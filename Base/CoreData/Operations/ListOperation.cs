using System;
using System.Threading.Tasks;
using CoreData.Infrastructure;
using CoreType.Types;

namespace CoreData.Operations
{
    public class ListOperation<TEntity, TRepository> : RepositoryOperationBaseAsync<Func<Task<PaginationWrapper<TEntity>>>, PaginationWrapper<TEntity>, TRepository>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase
    {
        public ListOperation(TRepository repository) : base(repository)
        {
        }

        protected override async Task<ResponseWrapper<PaginationWrapper<TEntity>>> OnExecuteAsync(Func<Task<PaginationWrapper<TEntity>>> listFunc, ResponseWrapper<PaginationWrapper<TEntity>> response)
        {
            PaginationWrapper<TEntity> result = default;

            try
            {
                result = await listFunc.Invoke();
            }
            catch (Exception e)
            {
                response.AddError(e);
            }

            if (result == null)
            {
                //TODO append to response.Errors                
                response.Message = LocalizedMessages.PROCESS_FAILED;
            }
            else
            {
                response.Data = result;
                response.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
                response.Success = true;
            }

            return response;
        }
    }
}