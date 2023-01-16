using System.Collections.Generic;
using System.Threading.Tasks;
using CoreType.Types;
using FluentValidation;

namespace CoreData.Infrastructure
{
    public interface IGenericServiceBasic<TEntity, TKey, out TRepository> : IGenericService<TEntity, TRepository> where TEntity : class, new()
    {
        ResponseWrapper<TEntity> GetById(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false);
        Task<ResponseWrapper<TEntity>> GetByIdAsync(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false);
        ResponseWrapper<bool> DeleteById(TKey primaryId);
        Task<ResponseWrapper<bool>> DeleteByIdAsync(TKey primaryId);
    }

    public interface IGenericService<TEntity, out TRepository> : IGenericService where TEntity : class, new()
    {
        ResponseWrapper<IList<TEntity>> GetAll(bool noTracking = true, bool ignoreQueryFilters = false);
        Task<ResponseWrapper<IList<TEntity>>> GetAllAsync(bool noTracking = true, bool ignoreQueryFilters = false);
        ResponseWrapper<PaginationWrapper<TEntity>> List(RequestWithPagination<TEntity> request);
        Task<ResponseWrapper<PaginationWrapper<TEntity>>> ListAsync(RequestWithPagination<TEntity> request);
        ResponseWrapper<TEntity> Get(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false);
        Task<ResponseWrapper<TEntity>> GetAsync(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false);
        ResponseWrapper BulkSave(RequestWithExcelData<TEntity> request);
        Task<ResponseWrapper> BulkSaveAsync(RequestWithExcelData<TEntity> request);
        ResponseWrapper BulkSave<TCustomValidator>(RequestWithExcelData<TEntity> request) where TCustomValidator : IValidator<TEntity>, new();
        Task<ResponseWrapper> BulkSaveAsync<TCustomValidator>(RequestWithExcelData<TEntity> request) where TCustomValidator : IValidator<TEntity>, new();
        ResponseWrapper BulkSave(RequestWithExcelData<TEntity> request, IValidator<TEntity> validator, bool ignoreValidation = false);
        Task<ResponseWrapper> BulkSaveAsync(RequestWithExcelData<TEntity> request, IValidator<TEntity> validator, bool ignoreValidation = false);
        ResponseWrapper<bool> Delete(TEntity entity);
        Task<ResponseWrapper<bool>> DeleteAsync(TEntity entity);
    }

    public interface IGenericService
    {
    }
}