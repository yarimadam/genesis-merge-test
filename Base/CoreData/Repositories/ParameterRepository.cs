using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;
using CoreType.Types;
using EFCoreSecondLevelCacheInterceptor;

namespace CoreData.Repositories
{
    public class ParameterRepository : GenericGenesisRepository<CoreParameters, int, CoreParametersValidator>
    {
        public ParameterRepository()
        {
        }

        public ParameterRepository(GenesisContextBase context) : base(context)
        {
        }

        public string GetLocalizedParam(string keyCode, string lang) => GetLocalizedParam(keyCode, null, lang);

        public string GetLocalizedParam(string keyCode, string val, string lang)
        {
            var parameterQuery = DbSet()
                .Where(x => x.KeyCode == keyCode);

            if (!string.IsNullOrEmpty(val))
                parameterQuery = parameterQuery
                    .Where(x => x.Value == val);

            var parameter = parameterQuery
                .Cacheable(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(5))
                .PrioritizeByTenant()
                .FirstOrDefault();

            if (parameter?.Translations != null)
            {
                var value = parameter.Translations.GetType().GetProperty(lang.ToUpper())?.GetValue(parameter.Translations, null);
                return value?.ToString();
            }

            return null;
        }

        public List<CoreParameters> GetAllLabels()
        {
            // Get last overridden parameters according to tenant/parentTenant (Client side evaluation)
            return DbSet(true, true)
                .OrderBy(p => p.KeyCode)
                .ThenBy(p => p.OrderIndex)
                .PrioritizeByTenant()
                .ToList();
        }

        public IList<CoreParameters> GetByKey(CoreParameters parameters)
        {
            parameters.Status ??= 1;

            return DbSet()
                .Where(x => x.KeyCode == parameters.KeyCode)
                .AddFilters(parameters)
                .OrderBy(x => x.OrderIndex)
                .PrioritizeByTenant()
                .ToList();
        }

        public IList<CoreParameters> CacheAllParameters()
        {
            var cacheExpiration = DistributedCache.Get("CacheExpiration_Parameters", true, false);
            if (cacheExpiration == null || DateTime.Parse(cacheExpiration) < DateTime.UtcNow)
            {
                DistributedCache.Set("CacheExpiration_Parameters", DateTime.UtcNow.Add(TimeSpan.FromHours(24)).ToString(CultureInfo.InvariantCulture));

                var request = new RequestWithPagination<CoreParameters> { Pagination = { MaxRowsPerPage = 9999 } };
                var paramList = List(request).List;

                foreach (var param in paramList)
                {
                    if (param.Translations != null)
                    {
                        DistributedCache.Set(param.KeyCode, param.Translations);
                    }
                }

                return paramList;
            }
            else
                return null;
        }
    }
}