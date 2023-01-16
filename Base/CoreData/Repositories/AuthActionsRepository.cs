using System;
using System.Linq;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;
using CoreType.Types;
using FluentValidation;
using Serilog;

namespace CoreData.Repositories
{
    public class AuthActionsRepository : GenericGenesisRepository<AuthActions, int>
    {
        public AuthActionsRepository()
        {
        }

        public AuthActionsRepository(GenesisContextBase context) : base(context)
        {
        }

        public override async Task<AuthActions> SaveAsync(AuthActions entity, IValidator<AuthActions> validator, bool ignoreValidation = false)
        {
            AuthActions oldRecord = null;
            if (entity.ActionId > 0)
                oldRecord = await GetAsync(entity, true);

            await base.SaveAsync(entity, validator, ignoreValidation);

            // Clear cached claims if resource is no longer active. 
            if (oldRecord != null
                && oldRecord.Status == (int) Status.Active
                && entity.Status != (int) Status.Active)
                await DistributedCache.ClearAllClaimsAsync();

            return entity;
        }

        public override async Task<bool> DeleteAsync(AuthActions entity)
        {
            await using var tran = await Context.Database.BeginTransactionAsync();

            try
            {
                await DbSet<AuthUserRights>()
                    .Where(x => x.ActionId == entity.ActionId)
                    .DeleteFromQueryAsync();

                await base.DeleteAsync(entity);

                await tran.CommitAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "AuthActionsRepository.DeleteAsync");
                await tran.RollbackAsync();
                throw;
            }

            return true;
        }
    }
}