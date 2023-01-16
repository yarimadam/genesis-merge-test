using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;
using CoreType.Types;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoreData.Repositories
{
    public class CommunicationDefinitionsRepository : GenericGenesisRepository<CommunicationDefinitions, int, CommunicationDefinitionsValidator>
    {
        private static readonly CommunicationTemplatesRepository _communicationTemplatesRepository = new CommunicationTemplatesRepository();

        public CommunicationDefinitionsRepository()
        {
        }

        public CommunicationDefinitionsRepository(GenesisContextBase context) : base(context)
        {
        }

        public async Task<PaginationWrapper<CommunicationDefinitions>> ListWithOutPasswords(RequestWithPagination<CommunicationDefinitions> entity)
        {
            return await ListAsQueryable(entity)
                .SelectExclusively(x => new { x.EmailPassword, x.SmsPassword })
                .ToPaginatedListAsync(entity);
        }

        public override async Task<CommunicationDefinitions> GetAsync(CommunicationDefinitions entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await GetAsQueryable(entity, noTracking, ignoreQueryFilters)
                .SelectExclusively(x => new { x.EmailPassword, x.SmsPassword })
                .FirstOrDefaultAsync();
        }

        public override async Task<CommunicationDefinitions> SaveAsync(CommunicationDefinitions entity, IValidator<CommunicationDefinitions> validator, bool ignoreValidation = false)
        {
            if (!ignoreValidation)
                await ValidateAndThrowAsync(entity, validator);

            bool isNewRecord = entity.CommDefinitionId <= 0;

            if (!isNewRecord)
            {
                var oldRecord = GetByIdAsQueryable(entity.CommDefinitionId, true)
                    .Select(x => new CommunicationDefinitions
                    {
                        EmailPassword = x.EmailPassword,
                        SmsPassword = x.SmsPassword
                    })
                    .Single();

                if (string.IsNullOrEmpty(entity.EmailPassword))
                    entity.EmailPassword = oldRecord.EmailPassword;
                else
                    entity.EmailPassword = EncryptionManager.Encrypt(entity.EmailPassword);

                if (string.IsNullOrEmpty(entity.SmsPassword))
                    entity.SmsPassword = oldRecord.SmsPassword;
                else
                    entity.SmsPassword = EncryptionManager.Encrypt(entity.SmsPassword);
            }
            else
            {
                entity.EmailPassword = EncryptionManager.Encrypt(entity.EmailPassword);
                entity.SmsPassword = EncryptionManager.Encrypt(entity.SmsPassword);
            }

            await base.SaveAsync(entity, null, true);

            await DistributedCache.DeleteAsync("CommunicationDefinitions");

            entity.EmailPassword = entity.SmsPassword = string.Empty;

            return entity;
        }

        public override async Task<bool> DeleteAsync(CommunicationDefinitions entity)
        {
            var result = await base.DeleteAsync(entity);

            await DistributedCache.DeleteAsync("CommunicationDefinitions");

            return result;
        }

        public IList<CommunicationDefinitions> CacheCommunicationDefinitions(bool externallyCached = false)
        {
            var defRequest = new RequestWithPagination<CommunicationDefinitions>
            {
                Criteria = { Status = 1 },
                Pagination = { MaxRowsPerPage = 9999 }
            };

            var defList = List(defRequest).List;

            if (!externallyCached)
                DistributedCache.Set("CommunicationDefinitions", JsonConvert.SerializeObject(defList));

            return defList;
        }

        public IList<CommunicationTemplates> CacheCommunicationTemplates(bool externallyCached = false)
        {
            var templateRequest = new RequestWithPagination<CommunicationTemplates>
            {
                Criteria = { Status = 1 },
                Pagination = { MaxRowsPerPage = 9999 }
            };

            var templateList = _communicationTemplatesRepository.List(templateRequest).List;

            if (!externallyCached)
                DistributedCache.Set("CommunicationTemplates", JsonConvert.SerializeObject(templateList));

            return templateList;
        }
    }
}