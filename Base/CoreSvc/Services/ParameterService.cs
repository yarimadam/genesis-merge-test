using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;
using CoreType.Types;
using FluentValidation;

namespace CoreSvc.Services
{
    public class ParameterService : GenericService<CoreParameters, int, CoreParametersValidator, ParameterRepository>
    {
        public override async Task<ResponseWrapper<CoreParameters>> SaveAsync(CoreParameters entity, IValidator<CoreParameters> validator, bool ignoreValidation = false)
        {
            var response = await base.SaveAsync(entity, validator, ignoreValidation);

            if (response.Success)
                DistributedCache.SetAsync(response.Data.KeyCode, response.Data.Translations);

            return response;
        }
    }
}