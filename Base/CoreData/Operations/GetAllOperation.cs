using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData.Infrastructure;
using CoreType.Types;

namespace CoreData.Operations
{
    public class GetAllOperation<TEntity, TRepository> : RepositoryOperationBaseAsync<Func<Task<IList<TEntity>>>, IList<TEntity>, TRepository>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase
    {
        public GetAllOperation(TRepository repository) : base(repository)
        {
        }

        protected override async Task<ResponseWrapper<IList<TEntity>>> OnExecuteAsync(Func<Task<IList<TEntity>>> getAllFunc, ResponseWrapper<IList<TEntity>> response)
        {
            IList<TEntity> result = default;

            try
            {
                result = await getAllFunc.Invoke();
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