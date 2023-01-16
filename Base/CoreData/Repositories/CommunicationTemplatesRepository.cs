using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;
using FluentValidation;
using CoreHelper = CoreData.Common.Helper;

namespace CoreData.Repositories
{
    public class CommunicationTemplatesRepository : GenericGenesisRepository<CommunicationTemplates, int, CommunicationTemplatesValidator>
    {
        public CommunicationTemplatesRepository()
        {
        }

        public CommunicationTemplatesRepository(GenesisContextBase context) : base(context)
        {
        }

        public override async Task<CommunicationTemplates> SaveAsync(CommunicationTemplates entity, IValidator<CommunicationTemplates> validator, bool ignoreValidation = false)
        {
            var result = await base.SaveAsync(entity, validator, ignoreValidation);

            await DistributedCache.DeleteAsync("CommunicationTemplates");

            return result;
        }

        public override async Task<bool> DeleteAsync(CommunicationTemplates entity)
        {
            var result = await base.DeleteAsync(entity);

            await DistributedCache.DeleteAsync("CommunicationTemplates");

            return result;
        }
    }
}