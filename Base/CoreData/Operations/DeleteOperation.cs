using System;
using System.Threading.Tasks;
using CoreData.Infrastructure;
using CoreType.Types;

namespace CoreData.Operations
{
    public class DeleteOperation<TEntity, TRepository> : RepositoryOperationBaseAsync<Func<Task<bool>>, bool, TRepository>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase
    {
        public DeleteOperation(TRepository repository) : base(repository)
        {
        }

        protected override async Task<ResponseWrapper<bool>> OnExecuteAsync(Func<Task<bool>> deleteFunc, ResponseWrapper<bool> response)
        {
            bool result = default;

            try
            {
                result = await deleteFunc.Invoke();
            }
            catch (Exception e)
            {
                response.AddError(e);
            }

            if (!result)
            {
                //TODO append to response.Errors  
                // response.AddError(LocalizedMessages.PROCESS_FAILED);
                response.Message = LocalizedMessages.PROCESS_FAILED;
            }
            else
            {
                response.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
                response.Success = response.Data = true;
            }

            return response;
        }
    }
}