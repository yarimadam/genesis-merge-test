using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;
using CoreType.Types;
using EFCoreSecondLevelCacheInterceptor;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CoreData.Repositories
{
    public class TenantRepository : GenericGenesisRepository<Tenant, int, TenantValidator>
    {
        public TenantRepository()
        {
        }

        public TenantRepository(GenesisContextBase context) : base(context)
        {
        }

        // TODO Evaluate
        public TenantRepository(SessionContext sessionContext) : base(sessionContext)
        {
        }

        public override async Task<PaginationWrapper<Tenant>> ListAsync(RequestWithPagination<Tenant> request)
        {
            var parentTenantList = await GetAllAsync();
            var isOwner = Session?.CurrentUser.TenantType == (int) TenantType.SystemOwner;
            var query = GetAllAsQueryable(true, isOwner);

            var res = await query
                .AddFilters(request)
                .AddSortings(request)
                .ThenByDescending(x => x.TenantId)
                .ToPaginatedListAsync(request);

            res.List = res.List.Select(x =>
                {
                    x.ParentTenantName = parentTenantList.Where(y => y.TenantId == x.ParentTenantId).Select(y => y.TenantName).FirstOrDefault();
                    return x;
                }
            ).ToList();

            return res;
        }

        public override async Task<IList<Tenant>> GetAllAsync(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            return await base.GetAllAsQueryable(noTracking, ignoreQueryFilters)
                .SelectExclusively(x => new { x.ParentTenant })
                .Cacheable(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(5))
                .ToListAsync();
        }

        public List<Tenant> ListSubTenants(int tenantId)
        {
            var tenants = GetAllAsQueryable(true, true)
                .Select(x => new Tenant
                {
                    TenantId = x.TenantId,
                    TenantType = x.TenantType,
                    ParentTenantId = x.ParentTenantId
                })
                .ToList();

            return RecurseSubTenants(tenants, tenantId).ToList();
        }

        public List<Tenant> ListParentTenants(int tenantId)
        {
            var tenants = GetAllAsQueryable(true, true)
                .Select(x => new Tenant
                {
                    TenantId = x.TenantId,
                    TenantType = x.TenantType,
                    ParentTenantId = x.ParentTenantId
                })
                .OrderByDescending(x => x.TenantId)
                .ToList();

            var parentTenants = RecurseParentTenants(tenants, tenantId).ToList();
            parentTenants = parentTenants.Where(x => x.TenantId != tenantId).ToList(); //To remove main tenant from the parent tenant list

            return parentTenants;
        }

        public List<Tenant> ListTenantAndSubTenants(int tenantId)
        {
            var userId = Session.GetUserId();

            var isSameTenant = DbSet<CoreUsers>(true, true)
                .Any(x => x.UserId == userId && x.TenantId == tenantId);

            if (!isSameTenant)
                throw new GenesisException(LocalizedMessages.ACCESS_DENIED_RECORD_BELONGS_TO_ANOTHER_TENANT);

            var tenants = GetAllAsQueryable(true, true)
                .Select(x => new Tenant
                {
                    TenantId = x.TenantId,
                    TenantName = x.TenantName,
                    TaxNumber = x.TaxNumber,
                    TenantType = x.TenantType,
                    ParentTenantId = x.ParentTenantId
                })
                .ToList();

            Tenant tenant = tenants.FirstOrDefault(x => x.TenantId == tenantId);
            List<Tenant> subTenants;

            if (Session?.CurrentUser.TenantType != (int) TenantType.SystemOwner)
            {
                subTenants = RecurseSubTenants(tenants, tenantId).Take(10).ToList();

                if (tenant != null)
                    subTenants.Insert(0, tenant);
            }
            else
            {
                subTenants = tenants;

                if (tenant != null)
                {
                    //user tenant to be first
                    subTenants = subTenants.Where(x => x.TenantId != tenant.TenantId).ToList();
                    subTenants.Insert(0, tenant);
                }

                subTenants = subTenants.Take(10).ToList();
            }

            return subTenants;
        }

        public List<Tenant> SearchTenantAndSubTenants(int tenantId, string searchString)
        {
            List<Tenant> subTenants = new List<Tenant>();

            if (string.IsNullOrEmpty(searchString))
            {
                return subTenants;
            }

            string upperSearchString = searchString.ToUpper(CultureInfo.InvariantCulture);

            var userId = Session.GetUserId();

            var isSameTenant = DbSet<CoreUsers>(true, true)
                .Any(x => x.UserId == userId && x.TenantId == tenantId);

            if (!isSameTenant)
                throw new GenesisException(LocalizedMessages.ACCESS_DENIED_RECORD_BELONGS_TO_ANOTHER_TENANT);

            var tenants = DbSet(true, true)
                .Select(x => new Tenant
                {
                    TenantId = x.TenantId,
                    TenantName = x.TenantName,
                    TaxNumber = x.TaxNumber,
                    TenantType = x.TenantType,
                    ParentTenantId = x.ParentTenantId
                })
                .Where(x => x.TenantName.ToUpper().Contains(upperSearchString) || x.TaxNumber.Contains(searchString))
                .ToList();

            Tenant tenant = tenants.FirstOrDefault(x => x.TenantId == tenantId);

            if (Session?.CurrentUser.TenantType != (int) TenantType.SystemOwner)
            {
                subTenants = RecurseSubTenants(tenants, tenantId).Take(10).ToList();

                if (tenant != null)
                    subTenants.Insert(0, tenant);
            }
            else
            {
                subTenants = tenants;

                if (tenant != null)
                {
                    //user tenant to be first
                    subTenants = subTenants.Where(x => x.TenantId != tenant.TenantId).Take(10).ToList();
                    subTenants.Insert(0, tenant);
                }

                subTenants = subTenants.Take(10).ToList();
            }

            return subTenants;
        }

        private static IList<Tenant> RecurseSubTenants(IList<Tenant> tenants, int tenantId)
        {
            var subTenants = tenants
                .Where(x => x.ParentTenantId != null && x.ParentTenantId == tenantId)
                .ToList();

            foreach (var subTenant in subTenants)
                subTenants = subTenants
                    .Concat(RecurseSubTenants(tenants, subTenant.TenantId))
                    .GroupBy(x => x.TenantId)
                    .Select(g => g.First())
                    .OrderBy(x => x.TenantId)
                    .ToList();

            return subTenants;
        }

        internal static IList<Tenant> RecurseParentTenants(IList<Tenant> tenants, int tenantId)
        {
            var parentTenants = tenants
                .Where(x => x.TenantId == tenantId)
                .ToList();

            foreach (var parentTenant in parentTenants)
            {
                if (parentTenant.ParentTenantId == null)
                    return parentTenants;

                parentTenants = parentTenants
                    .Concat(RecurseParentTenants(tenants, (int) parentTenant.ParentTenantId))
                    .GroupBy(x => x.TenantId)
                    .Select(g => g.First())
                    .ToList();
            }

            return parentTenants;
        }

        public override async Task<Tenant> GetAsync(Tenant entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            var isOwner = Session?.CurrentUser.TenantType == (int) TenantType.SystemOwner;

            return await GetAsQueryable(entity, true, isOwner)
                .Cacheable(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(1))
                .FirstOrDefaultAsync();
        }

        public override async Task<Tenant> SaveAsync(Tenant entity, IValidator<Tenant> validator, bool ignoreValidation = false)
        {
            if (!ignoreValidation)
                await ValidateAndThrowAsync(entity, validator);

            bool isNewRecord = entity.TenantId <= 0;
            Tenant defaultTenant = null;

            if (!string.IsNullOrEmpty(entity.Email))
            {
                var emailOwnerTenant = DbSet(false, true).Any(x => x.Email.Equals(entity.Email) && x.TenantId != entity.TenantId);

                if (emailOwnerTenant)
                    throw new GenesisException("EMAIL_ADDRESS_EXISTS_WARNING_MESSAGE", entity.Email);
            }

            if (isNewRecord)
                if (Session?.CurrentUser.TenantType != (int) TenantType.SystemOwner && Session.GetTenantId() != 0)
                    entity.ParentTenantId = Session.GetTenantId();

            if (entity.Status == (int) Status.Active && entity.IsDefault)
                defaultTenant = DbSet(true, true)
                    .FirstOrDefault(x => x.Status == (int) Status.Active && x.IsDefault);

            await base.SaveAsync(entity, null, true);

            if (entity.IsDefault && defaultTenant != null && entity.TenantId != defaultTenant.TenantId)
            {
                var updatedRows = await GetByIdAsQueryable(defaultTenant.TenantId)
                    .IgnoreQueryFilters()
                    .UpdateFromQueryAsync(x => new Tenant
                    {
                        IsDefault = false
                    });

                if (updatedRows == 0)
                    throw new GenesisException(LocalizedMessages.ERROR_SETTING_TENANT_AS_DEFAULT);
            }

            return entity;
        }
    }
}