using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;
using CoreType.Types;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using CoreHelper = CoreData.Common.Helper;

namespace CoreData.Repositories
{
    public class UserRepository : GenericGenesisRepository<CoreUsers, int, CoreUserValidator>
    {
        private static readonly AuthTemplatesRepository _authTemplatesRepository = new AuthTemplatesRepository();
        private static readonly TenantRepository _tenantRepository = new TenantRepository();

        public UserRepository()
        {
        }

        public UserRepository(GenesisContextBase context) : base(context)
        {
        }

        public override async Task<PaginationWrapper<CoreUsers>> ListAsync(RequestWithPagination<CoreUsers> request)
        {
            var tenantList = await _tenantRepository.GetAllAsync();
            var authTemplateList = await _authTemplatesRepository.GetAllAsync();

            var res = await ListAsQueryable(request)
                .SelectExclusively(x => new { x.Password, x.TempPassword, x.AuthUserRights, x.CoreDepartment, x.ForgotPasswordKey, x.VerificationKey })
                .ToPaginatedListAsync(request);

            res.List = res.List.Select(x =>
            {
                x.TenantName = tenantList.Where(y => y.TenantId == x.TenantId).Select(y => y.TenantName).FirstOrDefault();
                x.RoleName = authTemplateList.Where(z => z.AuthTemplateId == x.RoleId).Select(z => z.TemplateName).FirstOrDefault();
                return x;
            }).ToList();

            return res;
        }

        public override async Task<CoreUsers> GetAsync(CoreUsers entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            var user = await GetAsQueryable(entity, noTracking, ignoreQueryFilters)
                .SelectExclusively(x => new { x.Password, x.TempPassword, x.AuthUserRights, x.CoreDepartment, x.ForgotPasswordKey, x.VerificationKey })
                .FirstOrDefaultAsync();

            if (user != null)
            {
                //TODO Will be optimized
                user.TenantName = await DbSet<Tenant>(true, true)
                    .GetByIdAsQueryable(user.TenantId)
                    .Select(x => x.TenantName)
                    .FirstOrDefaultAsync();
            }

            return user;
        }

        public override async Task<CoreUsers> SaveAsync(CoreUsers entity, IValidator<CoreUsers> validator, bool ignoreValidation = false)
        {
            if (!ignoreValidation)
                await ValidateAndThrowAsync(entity, validator);

            bool isNewRecord = entity.UserId <= 0;

            if (DbSet(false, true).Any(x => x.Email.Equals(entity.Email) && x.UserId != entity.UserId))
                throw new GenesisException("EMAIL_ADDRESS_EXISTS_WARNING_MESSAGE", entity.Email);

            CoreUsers oldRecord = null;
            bool isRoleTemplate = false;
            AuthTemplate defaultAuthTemplate = null;
            bool isStatusChanged = false;

            if (!isNewRecord)
            {
                oldRecord = GetAsQueryable(entity, true)
                    .Select(x => new CoreUsers
                    {
                        Password = x.Password,
                        TempPassword = x.TempPassword,
                        RoleId = x.RoleId,
                        ForgotPasswordExpiration = x.ForgotPasswordExpiration,
                        ForgotPasswordKey = x.ForgotPasswordKey,
                        VerificationKey = x.VerificationKey,
                        VerificationKeyExpiration = x.VerificationKeyExpiration,
                        Status = x.Status
                    })
                    .Single();

                entity.RoleId = oldRecord.RoleId;
                entity.ForgotPasswordKey = oldRecord.ForgotPasswordKey;
                entity.ForgotPasswordExpiration = oldRecord.ForgotPasswordExpiration;
                entity.VerificationKey = oldRecord.VerificationKey;
                entity.VerificationKeyExpiration = oldRecord.VerificationKeyExpiration;

                if (entity.Status != oldRecord.Status)
                    isStatusChanged = true;
            }
            else
            {
                if (entity.Status != null)
                    isStatusChanged = true;

                entity.ForgotPasswordKey = null;
                entity.ForgotPasswordExpiration = null;
                entity.VerificationKey = null;
                entity.VerificationKeyExpiration = null;

                if (entity.RoleId == null || entity.RoleId <= 0)
                {
                    defaultAuthTemplate = _authTemplatesRepository.GetDefaultAuthTemplate(entity.TenantId);
                    if (defaultAuthTemplate != null)
                    {
                        isRoleTemplate = defaultAuthTemplate.AuthTemplateId > 0 && defaultAuthTemplate.TemplateType == (int) AuthTemplateType.Role;
                        if (isRoleTemplate)
                            entity.RoleId = defaultAuthTemplate.AuthTemplateId;
                    }
                }

                if (entity.Status == (int) UserStatus.WaitingForVerification)
                {
                    entity.VerificationKey = WebUtility.UrlEncode(Helper.GenerateUniqueKey());
                    entity.VerificationKeyExpiration = DateTime.UtcNow.Add(Constants.USER_VERIFICATION_EXPIRATION);
                }
            }

            if (!isNewRecord && string.IsNullOrEmpty(entity.Password))
            {
                entity.Password = oldRecord.Password;
                entity.TempPassword = oldRecord.TempPassword;
            }
            else
                entity.Password = CoreHelper.GetHashedString(entity.Password);

            await base.SaveAsync(entity, null, true);

            if (isNewRecord && defaultAuthTemplate?.AuthTemplateId > 0 && !isRoleTemplate)
            {
                var authRepository = new AuthRepository();
                var authorizationClaims = authRepository.GetAuthorizationClaims(entity.UserId, entity.RoleId);
                var userAuthorizationClaims = new UserAuthorizationClaims
                {
                    UserId = entity.UserId,
                    AuthorizationClaims = authorizationClaims,
                };

                authRepository.SaveAuthorizationClaims(userAuthorizationClaims);
            }

            if (entity.ShouldChangePassword)
                DistributedCache.ClearClaims(entity.UserId);

            if (isStatusChanged)
            {
                var genericResponse = new ResponseWrapper<CoreUsers> { Data = entity };

                if (entity.Status == (int) UserStatus.WaitingForVerification)
                    CommunicationManager.Mail.SendAsync("User Verification", entity, genericResponse);
                else if (isNewRecord)
                    CommunicationManager.Mail.SendAsync("Welcome User", entity, genericResponse);
            }

            entity.Password = entity.TempPassword = entity.ForgotPasswordKey = entity.VerificationKey = string.Empty;
            entity.ForgotPasswordExpiration = null;
            entity.VerificationKeyExpiration = null;

            return entity;
        }

        public override async Task<bool> DeleteAsync(CoreUsers entity)
        {
            entity = GetAsQueryable(entity)
                .Include(x => x.AuthUserRights)
                .First();

            await using var tran = await Context.Database.BeginTransactionAsync();

            try
            {
                DbSet<AuthUserRights>().RemoveRange(entity.AuthUserRights);

                await base.DeleteAsync(entity);

                await tran.CommitAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "UserRepository.DeleteAsync");

                await tran.RollbackAsync();

                throw;
            }

            return true;
        }

        public CoreUsers VerifyUser(CoreUsers parameters)
        {
            var query = DbSet(false, true)
                .Where(x => x.VerificationKey == parameters.VerificationKey
                            && x.Status == (int) UserStatus.WaitingForVerification);

            var user = query
                .Select(x => new CoreUsers
                {
                    UserId = x.UserId,
                    Name = x.Name,
                    Surname = x.Surname,
                    Email = x.Email,
                    VerificationKeyExpiration = x.VerificationKeyExpiration,
                })
                .FirstOrDefault();

            if (user != null)
            {
                if (user.VerificationKeyExpiration < DateTime.UtcNow)
                    throw new GenesisException(LocalizedMessages.USER_VERIFICATION_EXPIRED);

                var updatedRows = query.UpdateFromQuery(x => new CoreUsers
                {
                    Status = (int) UserStatus.StillWorks,
                    VerificationKey = null,
                    VerificationKeyExpiration = null
                });

                user.VerificationKeyExpiration = null;

                return updatedRows > 0 ? user : null;
            }
            else
                throw new GenesisException(LocalizedMessages.USER_VERIFICATION_INVALID_KEY);
        }
    }
}