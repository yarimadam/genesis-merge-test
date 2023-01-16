using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoreType.Attributes;
using CoreType.DBModels;
using CoreType.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace CoreData.Common
{
    public abstract class ContextBase : DbContext
    {
        public ContextBase()
        {
        }

        public ContextBase(DbContextOptions options)
            : base(options)
        {
        }

        public ContextBase(SessionContext session)
        {
            Session = session;
        }

        public ContextBase(DbContextOptions options, SessionContext session)
            : base(options)
        {
            Session = session;
        }

        #region Variables

        private SessionContext _session;

        public SessionContext Session
        {
            get => _session ??= SessionAccessor.GetSession();
            set => _session = value;
        }

        public static readonly ILoggerFactory LogFactory = new SerilogLoggerFactory();

        private MethodInfo _setGlobalQueryMethodInfo;

        private MethodInfo SetGlobalQueryMethodInfo
        {
            get
            {
                if (_setGlobalQueryMethodInfo == null)
                    _setGlobalQueryMethodInfo = GetType()
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .FirstOrDefault(t => t.IsGenericMethod && t.Name == nameof(SetGlobalQuery));

                return _setGlobalQueryMethodInfo;
            }
        }

        private MethodInfo _entityMethodInfo;

        private MethodInfo EntityMethodInfo
        {
            get
            {
                if (_entityMethodInfo == null)
                    _entityMethodInfo = typeof(ModelBuilder)
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(t => t.IsGenericMethod && t.Name == nameof(ModelBuilder.Entity));

                return _entityMethodInfo;
            }
        }

        #endregion

        #region Methods

        // TODO Compare logic with OnBeforeSaving then replace
        public int GetComputedTenantId<T>(T entity)
        {
            var entry = Entry(entity);
            var userId = Session.GetUserId();
            var tenantId = Session.GetTenantId();
            var tenantType = Session.GetTenantType();

            if (entity is ITenantInfo trackable1)
            {
                if (tenantType != null && (tenantType != (int) TenantType.SystemOwner || Session?.CurrentUser.XTenantId != null))
                    // TenantId value is set to current value if userId is 0, for register
                    return userId > 0 ? tenantId : trackable1.TenantId;
                if (tenantType == (int) TenantType.SystemOwner && !(entity is CoreUsers))
                    return (int) GetTenantOriginalValue(trackable1, entry);

                return trackable1.TenantId;
            }

            return default;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (SetGlobalQueryMethodInfo == null || EntityMethodInfo == null)
                return;

            var entityTypes = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                var clrType = entityType.ClrType;
                var properties = entityType
                    .GetProperties()
                    .Where(p => p.ClrType == typeof(string) && clrType.GetProperty(p.Name)?.GetCustomAttribute<PersistenceConverterAttribute>() != null);

                foreach (var property in properties)
                {
                    var converterAttribute = clrType.GetProperty(property.Name)?.GetCustomAttribute<PersistenceConverterAttribute>();
                    if (converterAttribute != null && property.GetValueConverter() == null)
                        property.SetValueConverter(converterAttribute.ToValueConverter());
                }

                var hasTenancyInterface = clrType.IsSubclassOf(typeof(TenantInfo)) || clrType.GetInterface(nameof(ITenantInfo)) != null;
                var hasSoftDeleteAttribute = clrType.GetCustomAttribute<SoftDeleteAttribute>() != null;

                if (!hasTenancyInterface && !hasSoftDeleteAttribute)
                    continue;

                var setGlobalQueryDel = SetGlobalQueryMethodInfo.MakeGenericMethod(entityType.ClrType);
                setGlobalQueryDel.Invoke(this, new object[] { modelBuilder });
            }
        }

        private void OnBeforeSaving()
        {
            var userId = Session.GetUserId();
            var tenantId = Session.GetTenantId();
            var tenantType = Session.GetTenantType();
            var subTenants = Session.GetSubTenantIds();

            var entries = ChangeTracker.Entries()
                .Where(entry => entry.State != EntityState.Unchanged)
                .ToList();

            foreach (var entry in entries)
            {
                if (entry.Entity is ITenantInfo trackable1)
                {
                    // Set current users tenantId if its not SystemOwner. Otherwise value stays as original value or 0.
                    if (tenantType != null && (tenantType != (int) TenantType.SystemOwner || Session?.CurrentUser.XTenantId != null))
                    {
                        // TenantId value is set to current value if userId is 0, for register
                        trackable1.TenantId = (int) (userId > 0 ? tenantId : entry.Property(nameof(TenantInfo.TenantId)).CurrentValue);

                        if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                        {
                            object originalValueRef = null;
                            var propertyEntry = entry.Property(nameof(TenantInfo.TenantId));

                            object GetTenantOriginalValue() => ContextBase.GetTenantOriginalValue(ref originalValueRef, trackable1, entry, propertyEntry);

                            if (entry.State == EntityState.Modified)
                            {
                                if (!(entry.Entity is Tenant) && !(entry.Entity is CoreUsers))
                                {
                                    //If tenantId changed somehow force update to insert instead.
                                    if (!propertyEntry.CurrentValue.Equals(GetTenantOriginalValue()))
                                    {
                                        entry.State = EntityState.Added;
                                        var primaryField = entry.Properties.FirstOrDefault(prop => prop.Metadata.IsPrimaryKey());
                                        if (primaryField != null)
                                        {
                                            var clrType = primaryField.Metadata.ClrType;
                                            primaryField.CurrentValue = Convert.ChangeType(0, Nullable.GetUnderlyingType(clrType) ?? clrType);
                                        }
                                    }
                                }
                            }
                            else if (entry.State == EntityState.Deleted)
                            {
                                if (!GetTenantOriginalValue().Equals(tenantId) && !subTenants.Contains(Convert.ToInt32(GetTenantOriginalValue())))
                                {
                                    if (entries.Count == 1)
                                        throw new GenesisException(LocalizedMessages.ACCESS_DENIED_DELETE_RECORD_BELONGS_TO_ANOTHER_TENANT);

                                    entry.State = EntityState.Unchanged;
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tenantType == (int) TenantType.SystemOwner && !(entry.Entity is CoreUsers))
                            entry.Property(nameof(TenantInfo.TenantId)).IsModified = false;
                    }
                }

                if (entry.Entity is IBaseContract trackable2)
                {
                    var now = DateTime.UtcNow;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            trackable2.CreatedDate = now;
                            trackable2.CreatedUserId = userId;
                            break;

                        case EntityState.Modified:
                            entry.Property("CreatedDate").IsModified = false;
                            entry.Property("CreatedUserId").IsModified = false;
                            trackable2.UpdatedDate = now;
                            trackable2.UpdatedUserId = userId;
                            break;

                        case EntityState.Deleted:
                            entry.Property("CreatedDate").IsModified = false;
                            entry.Property("CreatedUserId").IsModified = false;
                            trackable2.UpdatedDate = now;
                            trackable2.UpdatedUserId = userId;
                            break;
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private static object GetTenantOriginalValue(ITenantInfo trackable1, EntityEntry entry, PropertyEntry propertyEntry = null)
        {
            object originalValueRef = null;
            return GetTenantOriginalValue(ref originalValueRef, trackable1, entry);
        }

        private static object GetTenantOriginalValue(ref object originalValueRef, ITenantInfo trackable1, EntityEntry entry, PropertyEntry propertyEntry = null)
        {
            if (originalValueRef != null)
                return originalValueRef;

            propertyEntry ??= entry.Property(nameof(TenantInfo.TenantId));

            if (propertyEntry != null && !propertyEntry.OriginalValue.Equals(default(int)))
                return originalValueRef = propertyEntry.OriginalValue;

            var propertyValues = entry.GetDatabaseValues();
            if (propertyValues != null)
                return originalValueRef = propertyValues.GetValue<object>(nameof(TenantInfo.TenantId));

            return originalValueRef = trackable1.TenantId;
        }

        // To be able to use tenantId as a filter, constraint type should contain the property
        // Cannot be private
        // ReSharper disable once MemberCanBePrivate.Global
        protected void SetGlobalQuery<T>(ModelBuilder builder) where T : class
        {
            if (EntityMethodInfo == null)
                return;

            var type = typeof(T);
            var entityMethod = EntityMethodInfo.MakeGenericMethod(type);

            var entityTypeBuilderObj = entityMethod.Invoke(builder, new object[] { });
            if (entityTypeBuilderObj is EntityTypeBuilder<T> entityTypeBuilder)
            {
                const int systemOwner = (int) TenantType.SystemOwner;

                var hasTenancyInterface = type.IsSubclassOf(typeof(TenantInfo)) || type.GetInterface(nameof(ITenantInfo)) != null;
                var softDeleteAttribute = type.GetCustomAttribute<SoftDeleteAttribute>();
                var hasSoftDeleteAttribute = softDeleteAttribute != null;

                Expression<Func<T, bool>> mainFilterExpression = null;
                if (hasTenancyInterface)
                    mainFilterExpression = e =>
                        (Session.GetTenantType() == systemOwner
                         || EF.Property<int>(e, nameof(ITenantInfo.TenantId)) == 0
                         || Session.GetTenantId() == 0
                         || EF.Property<int>(e, nameof(ITenantInfo.TenantId)) == Session.GetTenantId()
                         || Session.GetSubTenantIds().Contains(EF.Property<int>(e, nameof(ITenantInfo.TenantId))))
                        && (
                            !hasSoftDeleteAttribute
                            || EF.Property<object>(e, softDeleteAttribute.PropertyName) != softDeleteAttribute.ValueToBeAssigned
                        );
                else if (hasSoftDeleteAttribute)
                    mainFilterExpression = e => EF.Property<object>(e, softDeleteAttribute.PropertyName) != softDeleteAttribute.ValueToBeAssigned;

                if (mainFilterExpression != null)
                    entityTypeBuilder.HasQueryFilter(mainFilterExpression);
            }
        }

        #endregion
    }
}