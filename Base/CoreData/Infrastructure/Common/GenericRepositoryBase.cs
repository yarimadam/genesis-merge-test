using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Validators;
using CoreType.Attributes;
using CoreType.Types;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CoreData.Infrastructure
{
    public abstract class GenericRepositoryBase<TEntity, TKey> : GenericRepositoryBase<TEntity>, IGenericRepository<TEntity, TKey>
        where TEntity : class, new()
    {
        #region Constructors

        protected GenericRepositoryBase(DbContext context, SessionContext session = null) : base(context, session)
        {
        }

        #endregion

        #region Get

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ IQueryable<TEntity> GetByIdAsQueryable(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return GetAsQueryable(ConvertToEntity(primaryId), noTracking, ignoreQueryFilters);
        }

        public /*virtual*/ TEntity GetById(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return GetByIdAsync(primaryId, noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<TEntity> GetByIdAsync(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await GetAsync(ConvertToEntity(primaryId), noTracking, ignoreQueryFilters);
        }

        #endregion

        #endregion

        #region Delete

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ bool DeleteById(TKey primaryId)
        {
            return DeleteByIdAsync(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> DeleteByIdAsync(TKey primaryId)
        {
            return await DeleteAsync(ConvertToEntity(primaryId));
        }

        #endregion

        #endregion

        #region SoftDelete

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ bool SoftDeleteById(TKey primaryId)
        {
            return SoftDeleteByIdAsync(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> SoftDeleteByIdAsync(TKey primaryId)
        {
            return await base.SoftDeleteByIdAsync(primaryId);
        }

        #endregion

        #endregion

        #region HardDelete

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ bool HardDeleteById(TKey primaryId)
        {
            return HardDeleteByIdAsync(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> HardDeleteByIdAsync(TKey primaryId)
        {
            return await base.HardDeleteByIdAsync(primaryId);
        }

        #endregion

        #endregion

        #region Helpers

        public new TKey GetPrimaryId(TEntity entity) => GetPrimaryId<TKey>(entity);

        public void SetPrimaryId(TEntity entity, TKey value) => SetPrimaryId<TKey>(entity, value);

        public void SetPropertyValue(TEntity entity, string propertyName, TKey value) => SetPropertyValue<TKey>(entity, propertyName, value);

        public TEntity ConvertToEntity(TKey primaryId) => ConvertToEntity<TKey>(primaryId);

        #endregion
    }

    public abstract class GenericRepositoryBase<TEntity> : GenericRepositoryBase, IGenericRepository<TEntity>
        where TEntity : class, new()
    {
        #region Variables

        public DbSet<TEntity> DbSet() => DbSet<TEntity>();
        public IQueryable<TEntity> DbSet(bool noTracking, bool ignoreQueryFilters = false) => DbSet<TEntity>(noTracking, ignoreQueryFilters);

        public IQueryable<TEntity> Query => Query<TEntity>();

        private SoftDeleteAttribute _softDeleteAttribute;
        public SoftDeleteAttribute SoftDeleteAttributeInstance => _softDeleteAttribute ??= SoftDeleteAttributeInstance<TEntity>();

        public bool HasSoftDeleteAttribute => SoftDeleteAttributeInstance != null;

        public virtual IValidator<RequestWithPagination<TEntity>> GetPaginationValidator() => base.GetPaginationValidator<TEntity>();

        public sealed override IValidator<RequestWithPagination<TCustomEntity>> GetPaginationValidator<TCustomEntity>()
        {
            if (typeof(TCustomEntity) == typeof(TEntity))
                return GetPaginationValidator() as IValidator<RequestWithPagination<TCustomEntity>>;

            return base.GetPaginationValidator<TCustomEntity>();
        }

        #endregion

        #region Constructors

        protected GenericRepositoryBase(DbContext context, SessionContext session = null) : base(context, session)
        {
        }

        #endregion

        #region GetAll

        #region Overrides

        public sealed override IQueryable<TCustomEntity> GetAllAsQueryable<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            if (typeof(TCustomEntity) == typeof(TEntity))
                return GetAllAsQueryable(noTracking, ignoreQueryFilters) as IQueryable<TCustomEntity>;

            return base.GetAllAsQueryable<TCustomEntity>(noTracking, ignoreQueryFilters);
        }

        public sealed override Task<IList<TCustomEntity>> GetAllAsync<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            return base.GetAllAsync<TCustomEntity>(noTracking, ignoreQueryFilters);
        }

        #endregion

        #region Extensions

        public virtual IQueryable<TEntity> GetAllAsQueryable(bool noTracking = true, bool ignoreQueryFilters = false)
            => base.GetAllAsQueryable<TEntity>(noTracking, ignoreQueryFilters);

        public /*virtual*/ IList<TEntity> GetAll(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            return GetAllAsync(noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<IList<TEntity>> GetAllAsync(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            return await base.GetAllAsync<TEntity>(noTracking, ignoreQueryFilters);
        }

        #endregion

        #endregion

        #region List

        #region Overrides

        public sealed override IOrderedQueryable<TCustomEntity> ListAsQueryable<TCustomEntity>(RequestWithPagination<TCustomEntity> request)
        {
            if (request is RequestWithPagination<TEntity> tempRequest)
                return ListAsQueryable(tempRequest) as IOrderedQueryable<TCustomEntity>;

            return base.ListAsQueryable(request);
        }

        public sealed override Task<PaginationWrapper<TCustomEntity>> ListAsync<TCustomEntity>(RequestWithPagination<TCustomEntity> request)
        {
            return base.ListAsync(request);
        }

        #endregion

        #region Extesions

        public virtual IOrderedQueryable<TEntity> ListAsQueryable(RequestWithPagination<TEntity> request)
        {
            return base.ListAsQueryable(request);
        }

        public /*virtual*/ PaginationWrapper<TEntity> List(RequestWithPagination<TEntity> request)
        {
            return ListAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<PaginationWrapper<TEntity>> ListAsync(RequestWithPagination<TEntity> request)
        {
            return await base.ListAsync(request);
        }

        #endregion

        #endregion

        #region Get

        #region Overrides

        public sealed override IQueryable<TCustomEntity> GetAsQueryable<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            if (entity is TEntity entityTemp)
                return GetAsQueryable(entityTemp, noTracking, ignoreQueryFilters) as IQueryable<TCustomEntity>;

            return base.GetAsQueryable(entity, noTracking, ignoreQueryFilters);
        }

        public sealed override Task<TCustomEntity> GetByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            if (typeof(TCustomEntity) == typeof(TEntity))
                return GetByIdAsync(primaryId, noTracking, ignoreQueryFilters) as Task<TCustomEntity>;

            return base.GetByIdAsync<TCustomEntity, TCustomKey>(primaryId, noTracking, ignoreQueryFilters);
        }

        #endregion

        #region Extensions

        public virtual IQueryable<TEntity> GetAsQueryable(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return base.GetAsQueryable(entity, noTracking, ignoreQueryFilters);
        }

        public /*virtual*/ IQueryable<TEntity> GetByIdAsQueryable<TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return GetAsQueryable(ConvertToEntity(primaryId), noTracking, ignoreQueryFilters);
        }

        public /*virtual*/ TEntity Get(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return GetAsync(entity, noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<TEntity> GetAsync(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await base.GetAsync(entity, noTracking, ignoreQueryFilters);
        }

        public /*virtual*/ async Task<TEntity> GetByIdAsync<TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await GetAsync(ConvertToEntity(primaryId), noTracking, ignoreQueryFilters);
        }

        #endregion

        #endregion

        #region Save

        #region Overrides

        public sealed override Task<TCustomEntity> SaveAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
        {
            if (entity is TEntity entityTemp)
                return SaveAsync(entityTemp, validator as IValidator<TEntity>, ignoreValidation) as Task<TCustomEntity>;

            return base.SaveAsync(entity, validator, ignoreValidation);
        }

        #endregion

        #region Extensions

        public /*virtual*/ TEntity Save(TEntity entity)
        {
            return SaveAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<TEntity> SaveAsync(TEntity entity)
        {
            return await SaveAsync(entity, null);
        }

        public /*virtual*/ TEntity Save<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new()
        {
            return SaveAsync<TCustomValidator>(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<TEntity> SaveAsync<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new()
        {
            return await SaveAsync(entity, new TCustomValidator());
        }

        public /*virtual*/ TEntity Save(TEntity entity, IValidator<TEntity> validator, bool ignoreValidation = false)
        {
            return SaveAsync(entity, validator, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<TEntity> SaveAsync(TEntity entity, IValidator<TEntity> validator, bool ignoreValidation = false)
        {
            return await base.SaveAsync(entity, validator, ignoreValidation);
        }

        #endregion

        #endregion

        #region Validations

        #region Overrides

        public sealed override Task<ValidationResult> ValidateAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator)
        {
            if (entity is TEntity entityTemp)
                return ValidateAsync(entityTemp, validator as IValidator<TEntity>);

            return base.ValidateAsync(entity, validator);
        }

        public sealed override Task ValidateAndThrowAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator)
        {
            if (entity is TEntity entityTemp)
                return ValidateAndThrowAsync(entityTemp, validator as IValidator<TEntity>);

            return base.ValidateAndThrowAsync(entity, validator);
        }

        #endregion

        #region Extensions

        public /*virtual*/ ValidationResult Validate<TCustomValidator>(TEntity entity)
            where TCustomValidator : IValidator<TEntity>, new()
        {
            return ValidateAsync<TCustomValidator>(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ValidationResult> ValidateAsync<TCustomValidator>(TEntity entity)
            where TCustomValidator : IValidator<TEntity>, new()
        {
            return await ValidateAsync(entity, new TCustomValidator());
        }

        public /*virtual*/ ValidationResult Validate(TEntity entity, IValidator<TEntity> validator)
        {
            return ValidateAsync(entity, validator).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ValidationResult> ValidateAsync(TEntity entity, IValidator<TEntity> validator)
        {
            return await base.ValidateAsync(entity, validator);
        }

        public /*virtual*/ void ValidateAndThrow<TCustomValidator>(TEntity entity)
            where TCustomValidator : IValidator<TEntity>, new()
        {
            ValidateAndThrowAsync<TCustomValidator>(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task ValidateAndThrowAsync<TCustomValidator>(TEntity entity)
            where TCustomValidator : IValidator<TEntity>, new()
        {
            await ValidateAndThrowAsync(entity, new TCustomValidator());
        }

        public /*virtual*/ void ValidateAndThrow(TEntity entity, IValidator<TEntity> validator)
        {
            ValidateAndThrowAsync(entity, validator).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task ValidateAndThrowAsync(TEntity entity, IValidator<TEntity> validator)
        {
            await base.ValidateAndThrowAsync(entity, validator);
        }

        #endregion

        #endregion

        #region Delete

        #region Overrides

        public sealed override async Task<bool> DeleteAsync<TCustomEntity>(TCustomEntity entity)
        {
            return await base.DeleteAsync(entity);
        }

        #endregion

        #region Extensions

        public /*virtual*/ bool Delete(TEntity entity)
        {
            return DeleteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            if (HasSoftDeleteAttribute)
                return await SoftDeleteAsync(entity);

            return await HardDeleteAsync(entity);
        }

        public /*virtual*/ bool DeleteById<TCustomKey>(TCustomKey primaryId)
        {
            return DeleteByIdAsync(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> DeleteByIdAsync<TCustomKey>(TCustomKey primaryId)
        {
            return await DeleteAsync(ConvertToEntity(primaryId));
        }

        #endregion

        #endregion

        #region SoftDelete

        #region Overrides

        public sealed override async Task<bool> SoftDeleteAsync<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class
        {
            return await base.SoftDeleteAsync(entity);
        }

        #endregion

        #region Extensions

        public /*virtual*/ bool SoftDeleteById<TCustomKey>(TCustomKey primaryId)
        {
            return SoftDeleteByIdAsync(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> SoftDeleteByIdAsync<TCustomKey>(TCustomKey primaryId)
        {
            return await SoftDeleteAsync(ConvertToEntity(primaryId));
        }

        public /*virtual*/ bool SoftDelete(TEntity entity)
        {
            return SoftDeleteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<bool> SoftDeleteAsync(TEntity entity)
        {
            return await base.SoftDeleteAsync(entity);
        }

        #endregion

        #endregion

        #region HardDelete

        #region Overrides

        public sealed override async Task<bool> HardDeleteAsync<TCustomEntity>(TCustomEntity entity)
        {
            return await base.HardDeleteAsync(entity);
        }

        #endregion

        #region Extensions

        public /*virtual*/ bool HardDeleteById<TCustomKey>(TCustomKey primaryId)
        {
            return HardDeleteByIdAsync(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> HardDeleteByIdAsync<TCustomKey>(TCustomKey primaryId)
        {
            return await HardDeleteAsync(ConvertToEntity(primaryId));
        }

        public /*virtual*/ bool HardDelete(TEntity entity)
        {
            return HardDeleteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<bool> HardDeleteAsync(TEntity entity)
        {
            return await base.HardDeleteAsync(entity);
        }

        #endregion

        #endregion

        #region Helpers

        public object GetPrimaryId(TEntity entity) => GetPrimaryId<object>(entity);

        public TKey GetPrimaryId<TKey>(TEntity entity) => GetPrimaryId<TEntity, TKey>(entity);

        public void SetPrimaryId<TKey>(TEntity entity, TKey value) => SetPrimaryId<TEntity, TKey>(entity, value);

        public void ClearPrimaryIds(TEntity entity) => ClearPrimaryIds<TEntity>(entity);

        public void SetPropertyValue<TKey>(TEntity entity, string propertyName, TKey value) => SetPropertyValue<TEntity, TKey>(entity, propertyName, value);

        public TEntity ConvertToEntity<TKey>(TKey primaryId) => ConvertToEntity<TEntity, TKey>(primaryId);

        #endregion
    }

    public abstract class GenericRepositoryBase : IGenericRepository
    {
        #region Variables

        private SessionContext _session;
        public SessionContext Session => _session ??= SessionAccessor.GetSession();

        public DbContext Context { get; }

        public DbSet<TCustomEntity> DbSet<TCustomEntity>()
            where TCustomEntity : class => Context.Set<TCustomEntity>();

        public IQueryable<TCustomEntity> DbSet<TCustomEntity>(bool noTracking, bool ignoreQueryFilters = false)
            where TCustomEntity : class
        {
            var query = Context.Set<TCustomEntity>().AsQueryable();

            if (noTracking)
                query = query.AsNoTracking();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            return query;
        }

        public IQueryable<TCustomEntity> Query<TCustomEntity>() where TCustomEntity : class => DbSet<TCustomEntity>();

        public SoftDeleteAttribute SoftDeleteAttributeInstance<TCustomEntity>()
            => typeof(TCustomEntity).GetCustomAttribute<SoftDeleteAttribute>();

        public bool HasSoftDeleteAttribute<TCustomEntity>()
            => SoftDeleteAttributeInstance<TCustomEntity>() != null;

        public virtual IValidator<RequestWithPagination<TCustomEntity>> GetPaginationValidator<TCustomEntity>()
            where TCustomEntity : class, new()
            => new PaginationValidator<TCustomEntity>();

        #endregion

        #region Constructors

        protected GenericRepositoryBase(DbContext context, SessionContext session = null)
        {
            _session = session;
            Context = context;
        }

        #endregion

        #region GetAll

        public virtual IQueryable<TCustomEntity> GetAllAsQueryable<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
            where TCustomEntity : class
        {
            return DbSet<TCustomEntity>(noTracking, ignoreQueryFilters);
        }

        public /*virtual*/ IList<TCustomEntity> GetAll<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
            where TCustomEntity : class
        {
            return GetAllAsync<TCustomEntity>(noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<IList<TCustomEntity>> GetAllAsync<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
            where TCustomEntity : class
        {
            return await GetAllAsQueryable<TCustomEntity>(noTracking, ignoreQueryFilters).ToListAsync();
        }

        #endregion

        #region List

        // TODO Pagination
        public virtual IOrderedQueryable<TCustomEntity> ListAsQueryable<TCustomEntity>(RequestWithPagination<TCustomEntity> request) where TCustomEntity : class, new()
        {
            return GetAllAsQueryable<TCustomEntity>()
                .AddFilters(request)
                .AddSortings(request);
        }

        public /*virtual*/ PaginationWrapper<TCustomEntity> List<TCustomEntity>(RequestWithPagination<TCustomEntity> request)
            where TCustomEntity : class, new()
        {
            return ListAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<PaginationWrapper<TCustomEntity>> ListAsync<TCustomEntity>(RequestWithPagination<TCustomEntity> request)
            where TCustomEntity : class, new()
        {
            var paginationValidator = GetPaginationValidator<TCustomEntity>();

            if (paginationValidator != null)
                await paginationValidator.ValidateAndThrowAsync(request);

            return await ListAsQueryable(request)
                .ToPaginatedListAsync(request);
        }

        #endregion

        #region Get

        public virtual IQueryable<TCustomEntity> GetAsQueryable<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            var primaryId = GetPrimaryId(entity);

            return GetByIdAsQueryable<TCustomEntity, object>(primaryId, noTracking, ignoreQueryFilters);
        }

        public /*virtual*/ IQueryable<TCustomEntity> GetByIdAsQueryable<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            return DbSet<TCustomEntity>(noTracking, ignoreQueryFilters).GetByIdAsQueryable(primaryId);
        }

        public /*virtual*/ TCustomEntity Get<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false) where TCustomEntity : class, new()
        {
            return GetAsync(entity, noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<TCustomEntity> GetAsync<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false) where TCustomEntity : class, new()
        {
            return await GetAsQueryable(entity, noTracking, ignoreQueryFilters).FirstOrDefaultAsync();
        }

        public /*virtual*/ TCustomEntity GetById<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false) where TCustomEntity : class, new()
        {
            return GetByIdAsync<TCustomEntity, TCustomKey>(primaryId, noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<TCustomEntity> GetByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            if (primaryId == null || primaryId.Equals(0) || primaryId.Equals(default))
                return null;

            return await GetByIdAsQueryable<TCustomEntity, TCustomKey>(primaryId, noTracking, ignoreQueryFilters).FirstOrDefaultAsync();
        }

        #endregion

        #region Save

        public /*virtual*/ TCustomEntity Save<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class
        {
            return SaveAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<TCustomEntity> SaveAsync<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class
        {
            return await SaveAsync(entity, null);
        }

        public /*virtual*/ TCustomEntity Save<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new() where TCustomEntity : class
        {
            return SaveAsync<TCustomEntity, TCustomValidator>(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<TCustomEntity> SaveAsync<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new() where TCustomEntity : class
        {
            return await SaveAsync(entity, new TCustomValidator());
        }

        public /*virtual*/ TCustomEntity Save<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
            where TCustomEntity : class
        {
            return SaveAsync(entity, validator, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<TCustomEntity> SaveAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
            where TCustomEntity : class
        {
            if (!ignoreValidation && validator != null)
                await ValidateAndThrowAsync(entity, validator);

            DbSet<TCustomEntity>().Update(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        #endregion

        #region Validations

        public /*virtual*/ ValidationResult Validate<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new()
        {
            return ValidateAsync<TCustomEntity, TCustomValidator>(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ValidationResult> ValidateAsync<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new()
        {
            return await ValidateAsync(entity, new TCustomValidator());
        }

        public /*virtual*/ ValidationResult Validate<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator)
        {
            return ValidateAsync(entity, validator).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ValidationResult> ValidateAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator)
        {
            if (validator != null)
                return await validator.ValidateAsync(entity);

            return new ValidationResult();
        }

        public /*virtual*/ void ValidateAndThrow<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new()
        {
            ValidateAndThrowAsync<TCustomEntity, TCustomValidator>(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task ValidateAndThrowAsync<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new()
        {
            await ValidateAndThrowAsync(entity, new TCustomValidator());
        }

        public /*virtual*/ void ValidateAndThrow<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator)
        {
            ValidateAndThrowAsync(entity, validator).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task ValidateAndThrowAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator)
        {
            ValidationResult validationResult = await ValidateAsync(entity, validator);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        #endregion

        #region Delete

        public /*virtual*/ bool DeleteById<TCustomEntity, TCustomKey>(TCustomKey primaryId)
            where TCustomEntity : class, new()
        {
            return DeleteByIdAsync<TCustomEntity, TCustomKey>(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> DeleteByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId)
            where TCustomEntity : class, new()
        {
            return await DeleteAsync(ConvertToEntity<TCustomEntity, TCustomKey>(primaryId));
        }

        public /*virtual*/ bool Delete<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            return DeleteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<bool> DeleteAsync<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            if (HasSoftDeleteAttribute<TCustomEntity>())
                return await SoftDeleteAsync(entity);

            return await HardDeleteAsync(entity);
        }

        #endregion

        #region SoftDelete

        public /*virtual*/ bool SoftDeleteById<TCustomEntity, TCustomKey>(TCustomKey primaryId)
            where TCustomEntity : class, new()
        {
            return SoftDeleteByIdAsync<TCustomEntity, TCustomKey>(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> SoftDeleteByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId)
            where TCustomEntity : class, new()
        {
            return await SoftDeleteAsync(ConvertToEntity<TCustomEntity, TCustomKey>(primaryId));
        }

        public /*virtual*/ bool SoftDelete<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            return SoftDeleteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<bool> SoftDeleteAsync<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            var primaryId = GetPrimaryId(entity);

            if (primaryId == null || primaryId.Equals(0) || primaryId.Equals(default))
                return false;

            if (!HasSoftDeleteAttribute<TCustomEntity>())
                throw new ArgumentException(nameof(SoftDeleteAttribute) + " is required on model to use soft delete feature !");

            var softDeleteAttribute = SoftDeleteAttributeInstance<TCustomEntity>();
            var updateExpression = EFCoreExtensions.CreateSoftDeleteExpression<TCustomEntity>(softDeleteAttribute);
            var affectedRows = await GetAsQueryable(entity).UpdateFromQueryAsync(updateExpression);

            var succeeded = affectedRows > 0;

            if (succeeded)
                SetPropertyValue(entity, softDeleteAttribute.PropertyName, softDeleteAttribute.ValueToBeAssigned);

            return succeeded;
        }

        #endregion

        #region HardDelete

        public /*virtual*/ bool HardDeleteById<TCustomEntity, TCustomKey>(TCustomKey primaryId)
            where TCustomEntity : class, new()
        {
            return HardDeleteByIdAsync<TCustomEntity, TCustomKey>(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<bool> HardDeleteByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId)
            where TCustomEntity : class, new()
        {
            return await HardDeleteAsync(ConvertToEntity<TCustomEntity, TCustomKey>(primaryId));
        }

        public /*virtual*/ bool HardDelete<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            return HardDeleteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<bool> HardDeleteAsync<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            var primaryId = GetPrimaryId(entity);

            if (primaryId == null || primaryId.Equals(0) || primaryId.Equals(default))
                return false;

            var affectedRows = await GetAsQueryable(entity).DeleteFromQueryAsync();

            var succeeded = affectedRows > 0;

            if (succeeded)
                ClearPrimaryIds(entity);

            return succeeded;
        }

        #endregion

        #region Helpers

        public object GetPrimaryId<TEntity>(TEntity entity)
            where TEntity : class => GetPrimaryId<TEntity, object>(entity);

        public TKey GetPrimaryId<TEntity, TKey>(TEntity entity)
            where TEntity : class => Context.Entry(entity).GetPrimaryIdVal<TKey>();

        public void SetPrimaryId<TEntity, TKey>(TEntity entity, TKey value)
            where TEntity : class => Context.Entry(entity).SetPrimaryIdVal(value);

        public void ClearPrimaryIds<TEntity>(TEntity entity)
            where TEntity : class => Context.Entry(entity).ClearPrimaryIds();

        public void SetPropertyValue<TEntity, TKey>(TEntity entity, string propertyName, TKey value)
            where TEntity : class => Context.Entry(entity).SetPropertyVal(propertyName, value);

        public TEntity ConvertToEntity<TEntity, TKey>(TKey primaryId)
            where TEntity : class, new()
        {
            var entity = new TEntity();

            SetPrimaryId(entity, primaryId);

            return entity;
        }

        #endregion
    }
}