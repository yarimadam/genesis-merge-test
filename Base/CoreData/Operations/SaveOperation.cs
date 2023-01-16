using System;
using System.Threading.Tasks;
using CoreData.Infrastructure;
using CoreType.Types;

namespace CoreData.Operations
{
    public class SaveOperation<TEntity, TRepository> : RepositoryOperationBaseAsync<Func<Task<TEntity>>, TEntity, TRepository>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase
    {
        public SaveOperation(TRepository repository) : base(repository)
        {
        }

        protected override async Task<ResponseWrapper<TEntity>> OnExecuteAsync(Func<Task<TEntity>> saveFunc, ResponseWrapper<TEntity> response)
        {
            TEntity result = default;

            try
            {
                result = await saveFunc.Invoke();

                if (result != null)
                {
                    var primaryId = Repository.GetPrimaryId(result);

                    if (primaryId == default)
                        result = null;
                }
            }
            // catch (DbUpdateConcurrencyException e)
            // {
            //     await e.Entries.Single().ReloadAsync();
            // }
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