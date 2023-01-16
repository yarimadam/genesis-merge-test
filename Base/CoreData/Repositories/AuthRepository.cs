using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;
using CoreType.Models;
using CoreType.Types;
using Microsoft.EntityFrameworkCore;
using CoreHelper = CoreData.Common.Helper;

namespace CoreData.Repositories
{
    public class AuthRepository : GenericGenesisRepository
    {
        private static readonly UserRepository _userRepository = new UserRepository();

        public AuthRepository()
        {
        }

        public AuthRepository(GenesisContextBase context) : base(context)
        {
        }

        public CoreUsers Login(LoggedInUser parameters)
        {
            var hashedPassword = CoreHelper.GetHashedString(parameters.Password);
            var user = DbSet<CoreUsers>()
                .Where(x => x.Email == parameters.Email && x.Password == hashedPassword)
                .Select(x => new CoreUsers
                {
                    Email = x.Email,
                    UserId = x.UserId,
                    Name = x.Name,
                    Surname = x.Surname,
                    Status = x.Status,
                })
                .FirstOrDefault();

            return user;
        }

        public AuthorizationClaims GetAuthorizationClaims(int userId, int? authTemplateId = null)
        {
            AuthorizationClaims authorizationClaims = new AuthorizationClaims();
            AuthTemplate template;

            var templateQuery = DbSet<AuthTemplate>(true, true);

            if (authTemplateId != null)
                template = templateQuery
                    .Where(x => x.AuthTemplateId == authTemplateId)
                    .Select(x => new AuthTemplate { AuthTemplateId = x.AuthTemplateId, TemplateType = x.TemplateType })
                    .FirstOrDefault();
            else
                template = (from u in DbSet<CoreUsers>(true, true)
                        join t in templateQuery
                            on u.RoleId equals t.AuthTemplateId
                        where u.UserId == userId && u.RoleId != null
                        select new AuthTemplate { AuthTemplateId = t.AuthTemplateId, TemplateType = t.TemplateType })
                    .FirstOrDefault();

            var isRoleTemplate = template != null && template.AuthTemplateId > 0 && template.TemplateType == (int) AuthTemplateType.Role;

            authorizationClaims.AuthResources = DbSet<AuthResources>(true, true)
                .Include(r => r.AuthActions)
                .ThenInclude(a => a.AuthUserRights.Where(u => u.UserId == userId))
                .ToList();

            var templateClaims = new List<AuthTemplateDetail>();

            if (isRoleTemplate || authTemplateId != null)
                templateClaims = DbSet<AuthTemplateDetail>(true, true)
                    .Where(d => d.AuthTemplateId == template.AuthTemplateId && d.Status == (int) Status.Active)
                    .ToList();

            //Update parent's statuses
            authorizationClaims.AuthResources = authorizationClaims.AuthResources
                .Select(r =>
                {
                    r.AuthActions = r.AuthActions
                        .Select(a =>
                        {
                            if (isRoleTemplate || authTemplateId != null)
                            {
                                var claim = templateClaims.Find(c => c.ResourceId == r.ResourceId && c.ActionId == a.ActionId);
                                if (claim != null)
                                {
                                    a.AuthUserRights.Add(new AuthUserRights
                                    {
                                        UserId = userId,
                                        ActionId = a.ActionId,
                                        Status = r.Status == (int) Status.Active && a.Status == (int) Status.Active ? claim.Status : (int) Status.Passive
                                    });
                                }
                            }

                            a.Status = a.Status == (int) Status.Active && a.AuthUserRights.Any(u => u.Status == (int) Status.Active) ? (int) Status.Active : (int) Status.Passive;
                            return a;
                        })
                        .ToList();

                    r.Status = r.Status == (int) Status.Active && r.AuthActions.Any(a => a.Status == (int) Status.Active) ? (int) Status.Active : (int) Status.Passive;
                    return r;
                })
                .ToList();

            return authorizationClaims;
        }

        public AuthorizationClaims GetAuthorizationClaimsSchema()
        {
            return new AuthorizationClaims
            {
                AuthResources = DbSet<AuthResources>(true)
                    .Include(p => p.AuthActions)
                    .OrderBy(x => x.OrderIndex)
                    .ToList()
            };
        }

        public UserAuthorizationClaims SaveAuthorizationClaims(UserAuthorizationClaims userAuthClaims)
        {
            Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            try
            {
                var isRoleTemplate = false;

                if (userAuthClaims.AuthTemplateId.HasValue)
                {
                    var templateType = DbSet<AuthTemplate>()
                        .GetByIdAsQueryable(userAuthClaims.AuthTemplateId)
                        .Select(x => x.TemplateType)
                        .Single();

                    isRoleTemplate = templateType == (int) AuthTemplateType.Role;
                }

                var userQuery = DbSet<CoreUsers>().GetByIdAsQueryable(userAuthClaims.UserId);

                // User has a role/template assigned so delete all custom rights.
                DbSet<AuthUserRights>()
                    .Where(x => x.UserId == userAuthClaims.UserId)
                    .DeleteFromQuery();

                if (isRoleTemplate)
                {
                    userQuery.UpdateFromQuery(x => new CoreUsers { RoleId = userAuthClaims.AuthTemplateId.Value });
                }
                else
                {
                    userQuery.UpdateFromQuery(x => new CoreUsers { RoleId = null });

                    var userRights = userAuthClaims
                        .AuthorizationClaims
                        .AuthResources
                        .SelectMany(x => x.AuthActions)
                        .SelectMany(x => x.AuthUserRights)
                        .Select(x =>
                        {
                            // Reset primary id
                            x.RightId = 0;
                            x.UserId = userAuthClaims.UserId;
                            return x;
                        })
                        .ToList();

                    DbSet<AuthUserRights>().AddRange(userRights);
                }

                Context.SaveChanges();

                return userAuthClaims;
            }
            finally
            {
                Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            }
        }

        public LoggedInUser ForgotPassword(LoggedInUser parameters)
        {
            ValidateAndThrow<LoggedInUser, ForgotPasswordValidator>(parameters);

            var query = DbSet<CoreUsers>(false, true)
                .Where(x => x.Email == parameters.Email && x.Status == 1);

            var user = query
                .Select(x => new LoggedInUser
                {
                    Email = x.Email
                })
                .FirstOrDefault();

            if (user != null)
            {
                var uniqueKey = user.ForgotPasswordKey = WebUtility.UrlEncode(CoreHelper.GenerateUniqueKey());

                var updatedRows = query.UpdateFromQuery(x => new CoreUsers
                {
                    ForgotPasswordKey = uniqueKey,
                    ForgotPasswordExpiration = DateTime.UtcNow.Add(Constants.RESET_PASSWORD_EXPIRATION)
                });

                return updatedRows > 0 ? user : null;
            }
            else
                throw new GenesisException("EMAIL_ADDRESS_NOT_EXISTS_WARNING_MESSAGE", parameters.Email);
        }

        public LoggedInUser ResetPassword(LoggedInUser parameters)
        {
            new ResetPasswordValidator().ValidateAndThrow(parameters);

            var query = DbSet<CoreUsers>(false, true)
                .Where(x => x.ForgotPasswordKey == parameters.ForgotPasswordKey
                            && x.Email == parameters.Email
                            && x.Status == 1);

            var user = query
                .Select(x => new CoreUsers
                {
                    UserId = x.UserId,
                    Name = x.Name,
                    Surname = x.Surname,
                    Email = x.Email,
                    ForgotPasswordExpiration = x.ForgotPasswordExpiration,
                })
                .FirstOrDefault();

            if (user != null)
            {
                if (user.ForgotPasswordExpiration < DateTime.UtcNow)
                    throw new GenesisException(LocalizedMessages.RESET_PASSWORD_EXPIRED);

                var hashedPass = CoreHelper.GetHashedString(parameters.Password);

                var updatedRows = query.UpdateFromQuery(x => new CoreUsers
                {
                    Password = hashedPass,
                    ForgotPasswordKey = null,
                    ForgotPasswordExpiration = null
                });

                return updatedRows > 0
                    ? new LoggedInUser
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        Surname = user.Surname,
                        Email = user.Email
                    }
                    : null;
            }
            else
                throw new GenesisException("EMAIL_ADDRESS_NOT_EXISTS_WARNING_MESSAGE", parameters.Email);
        }

        public string GetForgotPasswordEmail(LoggedInUser parameters)
        {
            var user = DbSet<CoreUsers>(false, true)
                .Where(x => x.ForgotPasswordKey == parameters.ForgotPasswordKey
                            && x.Status == 1)
                .Select(x =>
                    new CoreUsers
                    {
                        Email = x.Email,
                        ForgotPasswordExpiration = x.ForgotPasswordExpiration
                    })
                .FirstOrDefault();

            if (user?.ForgotPasswordExpiration == null || user.ForgotPasswordExpiration < DateTime.UtcNow)
                throw new GenesisException(LocalizedMessages.RESET_PASSWORD_EXPIRED);

            return user.Email;
        }

        public LoggedInUser ChangePassword(ChangePassword parameters)
        {
            new ChangePasswordValidator().ValidateAndThrow(parameters);

            var query = DbSet<CoreUsers>(false, true)
                .Where(x => x.Email == parameters.Email
                            && x.Status == 1);

            var user = query
                .Select(x => new CoreUsers
                {
                    UserId = x.UserId,
                    Name = x.Name,
                    Surname = x.Surname,
                    Email = x.Email,
                    Password = x.Password
                })
                .FirstOrDefault();

            if (user == null)
                throw new GenesisException("EMAIL_ADDRESS_NOT_EXISTS_WARNING_MESSAGE", parameters.Email);

            var hashedOldPass = CoreHelper.GetHashedString(parameters.CurrentPassword);
            if (user.Password != hashedOldPass)
                throw new GenesisException(LocalizedMessages.CURRENT_PASSWORD_IS_NOT_CORRECT);

            var hashedNewPass = CoreHelper.GetHashedString(parameters.NewPassword);

            var updatedRows = query.UpdateFromQuery(x => new CoreUsers
            {
                Password = hashedNewPass,
                ShouldChangePassword = false,
            });

            if (updatedRows <= 0)
                return null;

            DistributedCache.ClearClaims(user.UserId);

            return new LoggedInUser
            {
                UserId = user.UserId,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email
            };
        }

        public LoggedInUser ForceUserToChangePassword(LoggedInUser parameters)
        {
            var query = DbSet<CoreUsers>(false, true)
                .Where(x => x.UserId == parameters.UserId
                            && x.Email == parameters.Email
                            && x.Status == 1);

            var user = query
                .Select(x => new CoreUsers
                {
                    UserId = x.UserId,
                    Name = x.Name,
                    Surname = x.Surname,
                    Email = x.Email,
                })
                .FirstOrDefault();

            if (user == null)
                throw new GenesisException("EMAIL_ADDRESS_NOT_EXISTS_WARNING_MESSAGE", parameters.Email);

            var updatedRows = query.UpdateFromQuery(x => new CoreUsers
            {
                ShouldChangePassword = true,
            });

            if (updatedRows <= 0)
                return null;

            DistributedCache.ClearClaims(user.UserId);

            return new LoggedInUser
            {
                UserId = user.UserId,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                ShouldChangePassword = true
            };
        }

        public CoreUsers Register(CoreUsers entity)
        {
            new RegisterValidator().ValidateAndThrow(entity);

            var defaultTenant = DbSet<Tenant>(true, true)
                .FirstOrDefault(x => x.Status == (int) Status.Active && x.IsDefault);

            CoreUsers registerEntity = new CoreUsers
            {
                UserId = 0,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                Password = entity.Password,
                Status = (int) UserStatus.WaitingForVerification,
                TenantId = defaultTenant?.TenantId ?? 0,
            };

            _userRepository.Save(registerEntity);

            return registerEntity;
        }

        public bool ClearUserSessionsOfRole(long roleId)
        {
            var userIds = (from u in DbSet<CoreUsers>(true)
                    join t in DbSet<AuthTemplate>(true)
                        on u.RoleId equals t.AuthTemplateId
                    where u.RoleId == roleId && t.TemplateType == (int) AuthTemplateType.Role
                    select u.UserId)
                .ToArray();

            return DistributedCache.ClearClaims(userIds);
        }
    }
}