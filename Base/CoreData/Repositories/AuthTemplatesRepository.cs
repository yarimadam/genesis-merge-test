using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;
using CoreType.Types;
using EFCoreSecondLevelCacheInterceptor;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoreData.Repositories
{
    public class AuthTemplatesRepository : GenericGenesisRepository<AuthTemplate, int>
    {
        private static readonly AuthRepository _authRepository = new AuthRepository();
        private static readonly TenantRepository _tenantRepository = new TenantRepository();

        public AuthTemplatesRepository()
        {
        }

        public AuthTemplatesRepository(GenesisContextBase context) : base(context)
        {
        }

        public override async Task<IList<AuthTemplate>> GetAllAsync(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            return await base.GetAllAsQueryable(noTracking, ignoreQueryFilters)
                .Cacheable(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(5))
                .ToListAsync();
        }

        private string GetAuthTemplateDefaultPage(int userId)
        {
            var template = (
                    from u in DbSet<CoreUsers>(true)
                    join t in DbSet(true)
                        on u.RoleId equals t.AuthTemplateId
                    where u.UserId == userId && u.RoleId != null
                    select new { t.TemplateDefaultPage })
                .FirstOrDefault();

            return template?.TemplateDefaultPage;
        }

        public override async Task<AuthTemplate> GetAsync(AuthTemplate entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await GetAsQueryable(entity, noTracking, ignoreQueryFilters)
                .Include(x => x.AuthTemplateDetail)
                .FirstOrDefaultAsync();
        }

        // TODO Refactor - entity is always null on usages
        public AuthTemplate GetAssociatedTemplate(AuthTemplate entity, int userId = 0)
        {
            return userId > 0
                ? new AuthTemplate { TemplateDefaultPage = GetAuthTemplateDefaultPage(userId) }
                : GetById(entity.AuthTemplateId);
        }

        public override async Task<AuthTemplate> SaveAsync(AuthTemplate entity, IValidator<AuthTemplate> validator, bool ignoreValidation = false)
        {
            var isRoleInUse = false;
            AuthTemplate currentTemplate = null;

            if (entity.AuthTemplateId > 0)
            {
                var _authTemplateDetailsRepository = new AuthTemplateDetailsRepository(Context);

                _authTemplateDetailsRepository.DeletePassives(entity);

                currentTemplate = GetAsQueryable(entity, true).Single();

                if (currentTemplate.TemplateType == (int) AuthTemplateType.Role)
                {
                    isRoleInUse = IsRoleInUse(entity.AuthTemplateId);
                    if (isRoleInUse)
                    {
                        if (entity.TemplateType != (int) AuthTemplateType.Role)
                            throw new GenesisException(LocalizedMessages.ROLE_TYPE_CANNOT_CHANGE_ALREADY_USING);

                        if (entity.Status != 1)
                            throw new GenesisException(LocalizedMessages.ROLE_STATUS_CANNOT_CHANGE_ALREADY_USING);
                    }
                }
            }

            var isTenantChanged = currentTemplate != null && currentTemplate.TenantId != Context.GetComputedTenantId(entity);

            foreach (AuthTemplateDetail item in entity.AuthTemplateDetail)
            {
                if (entity.AuthTemplateId == 0 || item.AuthTemplateDetailId == 0 || isTenantChanged)
                {
                    if (entity.AuthTemplateId == 0)
                        item.AuthTemplateId = 0;

                    item.AuthTemplateDetailId = 0;

                    Context.Entry(item).State = EntityState.Added;
                }
                else
                    Context.Entry(item).State = EntityState.Unchanged;
            }

            if (entity.IsDefault)
            {
                var tenantId = Session.CurrentUser.XTenantId
                               ?? (Session.CurrentUser.TenantType == (int) TenantType.SystemOwner && currentTemplate != null
                                   ? currentTemplate.TenantId
                                   : Session.CurrentUser.TenantId);
                var oldDefaultTemplate = GetDefaultAuthTemplateByTenantId(tenantId, entity.AuthTemplateId);
                if (oldDefaultTemplate?.AuthTemplateId > 0)
                {
                    var updatedRows = await GetByIdAsQueryable(oldDefaultTemplate.AuthTemplateId)
                        .UpdateFromQueryAsync(x => new AuthTemplate { IsDefault = false });

                    if (updatedRows == 0)
                        throw new GenesisException(LocalizedMessages.ERROR_SETTING_TEMPLATE_AS_DEFAULT);
                }
            }

            await base.SaveAsync(entity, validator, ignoreValidation);

            if (isRoleInUse)
                _authRepository.ClearUserSessionsOfRole(entity.AuthTemplateId);

            return entity;
        }

        public bool IsRoleInUse(int authTemplateId)
        {
            return DbSet<CoreUsers>().Any(x => x.RoleId == authTemplateId);
        }

        public string GetTemplateName(int authTemplateId)
        {
            return GetByIdAsQueryable(authTemplateId)
                .Select(x => x.TemplateName)
                .Single();
        }

        public override async Task<bool> DeleteAsync(AuthTemplate entity)
        {
            Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            try
            {
                if (IsRoleInUse(entity.AuthTemplateId))
                    throw new GenesisException("ROLE_CANNOT_DELETE_ALREADY_USING");

                await using var tran = await Context.Database.BeginTransactionAsync();

                try
                {
                    await DbSet<AuthTemplateDetail>()
                        .Where(x => x.AuthTemplateId == entity.AuthTemplateId)
                        .DeleteFromQueryAsync();

                    await base.DeleteAsync(entity);

                    await tran.CommitAsync();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "AuthTemplatesRepository.Delete");
                    await tran.RollbackAsync();

                    throw;
                }

                return true;
            }
            finally
            {
                Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            }
        }

        public AuthTemplate GetDefaultAuthTemplateByTenantId(int tenantId, int? excludedAuthTemplateId = null)
        {
            var authTemplateQuery = DbSet(true, true)
                .Where(x => x.TenantId == tenantId && x.IsDefault && x.Status == (int) Status.Active);

            if (excludedAuthTemplateId > 0)
                authTemplateQuery = authTemplateQuery.Where(x => x.AuthTemplateId != excludedAuthTemplateId);

            return authTemplateQuery.FirstOrDefault();
        }

        public AuthTemplate GetDefaultAuthTemplate(int? tenantId)
        {
            tenantId ??= Session?.CurrentUser?.XTenantId ?? Session?.CurrentUser?.TenantId ?? 0;

            var defaultAuthTemplate = GetDefaultAuthTemplateByTenantId(tenantId.Value);
            if (defaultAuthTemplate != null || tenantId == 0)
                return defaultAuthTemplate;

            var authTemplates = DbSet(true, true)
                .Where(x => x.IsDefault && x.Status == (int) Status.Active)
                .ToList();

            if (authTemplates.Any())
            {
                var tenantAndChildTenants = _tenantRepository.ListParentTenants(tenantId.Value);
                var authTemplate = tenantAndChildTenants
                    .SelectMany(tenant => authTemplates.Where(template => template.TenantId == tenant.TenantId))
                    .FirstOrDefault();

                return authTemplate ?? authTemplates.FirstOrDefault(x => x.TenantId == 0);
            }

            return null;
        }
    }
}