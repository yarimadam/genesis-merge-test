using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Operations;
using CoreSvc.Operations;
using CoreType.Types;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CoreSvc.Services
{
    public abstract class GenericServiceBase<TEntity, TKey, TRepository> : GenericServiceBase<TEntity, TRepository>, IGenericServiceBasic<TEntity, TKey, TRepository>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase<TEntity, TKey>
    {
        protected GenericServiceBase(DbContext context = null) : base(context)
        {
        }

        #region Get

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ ResponseWrapper<TEntity> GetById(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return GetByIdAsync(primaryId, noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<TEntity>> GetByIdAsync(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await GetAsync(Repository.ConvertToEntity(primaryId), noTracking, ignoreQueryFilters);
        }

        #endregion

        #endregion

        #region Delete

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ ResponseWrapper<bool> DeleteById(TKey primaryId)
        {
            return DeleteByIdAsync(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<bool>> DeleteByIdAsync(TKey primaryId)
        {
            return await DeleteAsync(Repository.ConvertToEntity(primaryId));
        }

        #endregion

        #endregion
    }

    public abstract class GenericServiceBase<TEntity, TRepository> : GenericServiceBase<TRepository>, IGenericService<TEntity, TRepository>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase<TEntity>
    {
        protected GenericServiceBase(DbContext context = null) : base(context)
        {
        }

        #region GetAll

        #region Overrides

        public sealed override async Task<ResponseWrapper<IList<TCustomEntity>>> GetAllAsync<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            return await base.GetAllAsync<TCustomEntity>(noTracking, ignoreQueryFilters);
        }

        #endregion

        #region Extensions

        public /*virtual*/ ResponseWrapper<IList<TEntity>> GetAll(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            return GetAllAsync(noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<IList<TEntity>>> GetAllAsync(bool noTracking = true, bool ignoreQueryFilters = false)
        {
            return await new GetAllOperation<TEntity, TRepository>(Repository).ExecuteAsync(() => Repository.GetAllAsync(noTracking, ignoreQueryFilters));
        }

        #endregion

        #endregion

        #region List

        #region Overrides

        public sealed override async Task<ResponseWrapper<PaginationWrapper<TCustomEntity>>> ListAsync<TCustomEntity>(RequestWithPagination<TCustomEntity> request)
        {
            return await base.ListAsync(request);
        }

        #endregion

        #region Extesions

        public /*virtual*/ ResponseWrapper<PaginationWrapper<TEntity>> List(RequestWithPagination<TEntity> request)
        {
            return ListAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<PaginationWrapper<TEntity>>> ListAsync(RequestWithPagination<TEntity> request)
        {
            return await new ListOperation<TEntity, TRepository>(Repository).ExecuteAsync(() => Repository.ListAsync(request));
        }

        #endregion

        #endregion

        #region Get

        #region Overrides

        public sealed override Task<ResponseWrapper<TCustomEntity>> GetByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            if (typeof(TCustomEntity) == typeof(TEntity))
                return GetByIdAsync(primaryId, noTracking, ignoreQueryFilters) as Task<ResponseWrapper<TCustomEntity>>;

            return base.GetByIdAsync<TCustomEntity, TCustomKey>(primaryId, noTracking, ignoreQueryFilters);
        }

        #endregion

        #region Extensions

        public /*virtual*/ ResponseWrapper<TEntity> Get(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return GetAsync(entity, noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<TEntity>> GetAsync(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await new GetOperation<TEntity, TRepository>(Repository).ExecuteAsync(() => Repository.GetAsync(entity, noTracking, ignoreQueryFilters));
        }

        public /*virtual*/ async Task<ResponseWrapper<TEntity>> GetByIdAsync<TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await GetAsync(Repository.ConvertToEntity(primaryId), noTracking, ignoreQueryFilters);
        }

        #endregion

        #endregion

        #region Save

        #region Overrides

        public sealed override Task<ResponseWrapper<TCustomEntity>> SaveAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
        {
            if (entity is TEntity entityTemp)
                return SaveAsync(entityTemp, validator as IValidator<TEntity>, ignoreValidation) as Task<ResponseWrapper<TCustomEntity>>;

            return base.SaveAsync(entity, validator, ignoreValidation);
        }

        #endregion

        #region Extensions

        public /*virtual*/ ResponseWrapper<TEntity> Save(TEntity entity)
        {
            return SaveAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<TEntity>> SaveAsync(TEntity entity)
        {
            return await SaveAsync(entity, null);
        }

        public /*virtual*/ ResponseWrapper<TEntity> Save<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new()
        {
            return SaveAsync<TCustomValidator>(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<TEntity>> SaveAsync<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new()
        {
            return await SaveAsync(entity, new TCustomValidator());
        }

        public /*virtual*/ ResponseWrapper<TEntity> Save(TEntity entity, IValidator<TEntity> validator, bool ignoreValidation = false)
        {
            return SaveAsync(entity, validator, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<TEntity>> SaveAsync(TEntity entity, IValidator<TEntity> validator, bool ignoreValidation = false)
        {
            return await new SaveOperation<TEntity, TRepository>(Repository).ExecuteAsync(() => Repository.SaveAsync(entity, validator, ignoreValidation));
        }

        #endregion

        #endregion

        #region BulkSave

        #region Overrides

        public sealed override Task<ResponseWrapper> BulkSaveAsync<TCustomEntity>(RequestWithExcelData<TCustomEntity> request, IValidator<TCustomEntity> validator,
            bool ignoreValidation = false)
        {
            if (request is RequestWithExcelData<TEntity> entityTemp)
                return BulkSaveAsync(entityTemp, validator as IValidator<TEntity>, ignoreValidation);

            return base.BulkSaveAsync(request, validator, ignoreValidation);
        }

        #endregion

        public /*virtual*/ ResponseWrapper BulkSave(RequestWithExcelData<TEntity> request)
        {
            return BulkSaveAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper> BulkSaveAsync(RequestWithExcelData<TEntity> request)
        {
            return await BulkSaveAsync(request, null);
        }

        public /*virtual*/ ResponseWrapper BulkSave<TCustomValidator>(RequestWithExcelData<TEntity> request) where TCustomValidator : IValidator<TEntity>, new()
        {
            return BulkSaveAsync<TCustomValidator>(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper> BulkSaveAsync<TCustomValidator>(RequestWithExcelData<TEntity> request) where TCustomValidator : IValidator<TEntity>, new()
        {
            return await BulkSaveAsync(request, new TCustomValidator());
        }

        public /*virtual*/ ResponseWrapper BulkSave(RequestWithExcelData<TEntity> request, IValidator<TEntity> validator, bool ignoreValidation = false)
        {
            return BulkSaveAsync(request, validator, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper> BulkSaveAsync(RequestWithExcelData<TEntity> request, IValidator<TEntity> validator, bool ignoreValidation = false)
        {
            return await new BulkSaveOperation<TEntity, TRepository>(Repository, entity => Repository.SaveAsync(entity, validator, ignoreValidation)).ExecuteAsync(request);
        }

        #endregion

        #region Delete

        #region Overrides

        public sealed override Task<ResponseWrapper<bool>> DeleteAsync<TCustomEntity>(TCustomEntity entity)
        {
            return base.DeleteAsync(entity);
        }

        #endregion

        #region Extensions

        public /*virtual*/ ResponseWrapper<bool> Delete(TEntity entity)
        {
            return DeleteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<bool>> DeleteAsync(TEntity entity)
        {
            return await new DeleteOperation<TEntity, TRepository>(Repository).ExecuteAsync(() => Repository.DeleteAsync(entity));
        }

        public /*virtual*/ ResponseWrapper<bool> DeleteById<TCustomKey>(TCustomKey primaryId)
        {
            return DeleteByIdAsync(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<bool>> DeleteByIdAsync<TCustomKey>(TCustomKey primaryId)
        {
            return await DeleteAsync(Repository.ConvertToEntity(primaryId));
        }

        #endregion

        #endregion
    }

    public abstract class GenericServiceBase<TRepository> : IGenericService
        where TRepository : GenericRepositoryBase
    {
        private OperationManager _operationManager;
        protected OperationManager OperationManager => _operationManager ??= new OperationManager(UnitOfWork);

        protected readonly UnitOfWork UnitOfWork;

        public TRepository Repository { get; }

        protected GenericServiceBase(DbContext context = null)
        {
            Repository = TestHelpers.CreateRepository<TRepository>(context);
            UnitOfWork = new UnitOfWork(Repository.Context);
        }

        #region GetAll

        public /*virtual*/ ResponseWrapper<IList<TCustomEntity>> GetAll<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            return GetAllAsync<TCustomEntity>(noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<IList<TCustomEntity>>> GetAllAsync<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            return await new GetAllOperation<TCustomEntity, TRepository>(Repository).ExecuteAsync(() => Repository.GetAllAsync<TCustomEntity>(noTracking, ignoreQueryFilters));
        }

        #endregion

        #region List

        public /*virtual*/ ResponseWrapper<PaginationWrapper<TCustomEntity>> List<TCustomEntity>(RequestWithPagination<TCustomEntity> request)
            where TCustomEntity : class, new()
        {
            return ListAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<PaginationWrapper<TCustomEntity>>> ListAsync<TCustomEntity>(RequestWithPagination<TCustomEntity> request) where TCustomEntity : class, new()
        {
            return await new ListOperation<TCustomEntity, TRepository>(Repository).ExecuteAsync(() => Repository.ListAsync(request));
        }

        #endregion

        #region Get

        public /*virtual*/ ResponseWrapper<TCustomEntity> Get<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            return GetAsync(entity, noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<TCustomEntity>> GetAsync<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            return await new GetOperation<TCustomEntity, TRepository>(Repository).ExecuteAsync(() => Repository.GetAsync(entity, noTracking, ignoreQueryFilters));
        }

        public /*virtual*/ ResponseWrapper<TCustomEntity> GetById<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            return GetByIdAsync<TCustomEntity, TCustomKey>(primaryId, noTracking, ignoreQueryFilters).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<TCustomEntity>> GetByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new()
        {
            return await new GetOperation<TCustomEntity, TRepository>(Repository).ExecuteAsync(() => Repository.GetByIdAsync<TCustomEntity, TCustomKey>(primaryId, noTracking, ignoreQueryFilters));
        }

        #endregion

        #region Save

        public /*virtual*/ ResponseWrapper<TCustomEntity> Save<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            return SaveAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<TCustomEntity>> SaveAsync<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            return await SaveAsync(entity, null);
        }

        public /*virtual*/ ResponseWrapper<TCustomEntity> Save<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new() where TCustomEntity : class, new()
        {
            return SaveAsync<TCustomEntity, TCustomValidator>(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<TCustomEntity>> SaveAsync<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new() where TCustomEntity : class, new()
        {
            return await SaveAsync(entity, new TCustomValidator());
        }

        public /*virtual*/ ResponseWrapper<TCustomEntity> Save<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
            where TCustomEntity : class, new()
        {
            return SaveAsync(entity, validator, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<TCustomEntity>> SaveAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
            where TCustomEntity : class, new()
        {
            return await new SaveOperation<TCustomEntity, TRepository>(Repository).ExecuteAsync(() => Repository.SaveAsync(entity, validator, ignoreValidation));
        }

        #endregion

        #region BulkSave

        public /*virtual*/ ResponseWrapper BulkSave<TCustomEntity>(RequestWithExcelData<TCustomEntity> request)
            where TCustomEntity : class, new()
        {
            return BulkSaveAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper> BulkSaveAsync<TCustomEntity>(RequestWithExcelData<TCustomEntity> request)
            where TCustomEntity : class, new()
        {
            return await BulkSaveAsync(request, null);
        }

        public /*virtual*/ ResponseWrapper BulkSave<TCustomEntity, TCustomValidator>(RequestWithExcelData<TCustomEntity> request)
            where TCustomEntity : class, new()
            where TCustomValidator : IValidator<TCustomEntity>, new()
        {
            return BulkSaveAsync<TCustomEntity, TCustomValidator>(request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper> BulkSaveAsync<TCustomEntity, TCustomValidator>(RequestWithExcelData<TCustomEntity> request)
            where TCustomEntity : class, new()
            where TCustomValidator : IValidator<TCustomEntity>, new()
        {
            return await BulkSaveAsync(request, new TCustomValidator());
        }

        public /*virtual*/ ResponseWrapper BulkSave<TCustomEntity>(RequestWithExcelData<TCustomEntity> request, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
            where TCustomEntity : class, new()
        {
            return BulkSaveAsync(request, validator, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper> BulkSaveAsync<TCustomEntity>(RequestWithExcelData<TCustomEntity> request, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
            where TCustomEntity : class, new()
        {
            return await new BulkSaveOperation<TCustomEntity, TRepository>(Repository, entity => Repository.SaveAsync(entity, validator, ignoreValidation)).ExecuteAsync(request);
        }

        #endregion

        #region Delete

        public /*virtual*/ ResponseWrapper<bool> DeleteById<TCustomEntity, TCustomKey>(TCustomKey primaryId)
            where TCustomEntity : class, new()
        {
            return DeleteByIdAsync<TCustomEntity, TCustomKey>(primaryId).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<bool>> DeleteByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId)
            where TCustomEntity : class, new()
        {
            return await DeleteAsync(Repository.ConvertToEntity<TCustomEntity, TCustomKey>(primaryId));
        }

        public /*virtual*/ ResponseWrapper<bool> Delete<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            return DeleteAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<ResponseWrapper<bool>> DeleteAsync<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class, new()
        {
            return await new DeleteOperation<TCustomEntity, TRepository>(Repository).ExecuteAsync(() => Repository.DeleteAsync(entity));
        }

        #endregion
    }

    public abstract class GenericServiceBase : IGenericService
    {
        private OperationManager _operationManager;
        protected OperationManager OperationManager => _operationManager ??= new OperationManager(UnitOfWork);

        protected readonly UnitOfWork UnitOfWork;

        protected GenericServiceBase(DbContext context = null)
        {
            context ??= Helper.GetGenesisContext();
            UnitOfWork = new UnitOfWork(context);
        }
    }
}