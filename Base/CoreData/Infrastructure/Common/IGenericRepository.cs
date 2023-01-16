using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreType.Attributes;
using CoreType.Types;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CoreData.Infrastructure
{
    public interface IGenericRepository<TEntity, TKey> : IGenericRepository<TEntity> where TEntity : class, new()
    {
        IQueryable<TEntity> GetByIdAsQueryable(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false);
        TEntity GetById(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false);
        Task<TEntity> GetByIdAsync(TKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false);
        bool DeleteById(TKey primaryId);
        Task<bool> DeleteByIdAsync(TKey primaryId);
        bool SoftDeleteById(TKey primaryId);
        Task<bool> SoftDeleteByIdAsync(TKey primaryId);
        bool HardDeleteById(TKey primaryId);
        Task<bool> HardDeleteByIdAsync(TKey primaryId);

        new TKey GetPrimaryId(TEntity entity);
        void SetPrimaryId(TEntity entity, TKey value);
        TEntity ConvertToEntity(TKey primaryId);
    }

    public interface IGenericRepository<TEntity> : IGenericRepository where TEntity : class, new()
    {
        IQueryable<TEntity> Query { get; }
        SoftDeleteAttribute SoftDeleteAttributeInstance { get; }
        bool HasSoftDeleteAttribute { get; }
        DbSet<TEntity> DbSet();
        IQueryable<TEntity> DbSet(bool noTracking, bool ignoreQueryFilters = false);

        IQueryable<TEntity> GetAllAsQueryable(bool noTracking = true, bool ignoreQueryFilters = false);
        IList<TEntity> GetAll(bool noTracking = true, bool ignoreQueryFilters = false);
        Task<IList<TEntity>> GetAllAsync(bool noTracking = true, bool ignoreQueryFilters = false);

        IOrderedQueryable<TEntity> ListAsQueryable(RequestWithPagination<TEntity> request);
        PaginationWrapper<TEntity> List(RequestWithPagination<TEntity> request);
        Task<PaginationWrapper<TEntity>> ListAsync(RequestWithPagination<TEntity> request);

        IQueryable<TEntity> GetAsQueryable(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false);
        TEntity Get(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false);
        Task<TEntity> GetAsync(TEntity entity, bool noTracking = false, bool ignoreQueryFilters = false);

        TEntity Save(TEntity entity);
        Task<TEntity> SaveAsync(TEntity entity);
        TEntity Save<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new();
        Task<TEntity> SaveAsync<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new();
        TEntity Save(TEntity entity, IValidator<TEntity> validator, bool ignoreValidation = false);
        Task<TEntity> SaveAsync(TEntity entity, IValidator<TEntity> validator, bool ignoreValidation = false);

        bool Delete(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
        bool SoftDelete(TEntity entity);
        Task<bool> SoftDeleteAsync(TEntity entity);
        bool HardDelete(TEntity entity);
        Task<bool> HardDeleteAsync(TEntity entity);

        ValidationResult Validate(TEntity entity, IValidator<TEntity> validator);
        Task<ValidationResult> ValidateAsync(TEntity entity, IValidator<TEntity> validator);
        ValidationResult Validate<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new();
        Task<ValidationResult> ValidateAsync<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new();
        void ValidateAndThrow<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new();
        Task ValidateAndThrowAsync<TCustomValidator>(TEntity entity) where TCustomValidator : IValidator<TEntity>, new();
        void ValidateAndThrow(TEntity entity, IValidator<TEntity> validator);
        Task ValidateAndThrowAsync(TEntity entity, IValidator<TEntity> validator);

        object GetPrimaryId(TEntity entity);
        TKey GetPrimaryId<TKey>(TEntity entity);
        void SetPrimaryId<TKey>(TEntity entity, TKey value);
        TEntity ConvertToEntity<TKey>(TKey primaryId);
        IValidator<RequestWithPagination<TEntity>> GetPaginationValidator();
    }

    public interface IGenericRepository
    {
        SessionContext Session { get; }
        DbContext Context { get; }

        DbSet<TCustomEntity> DbSet<TCustomEntity>()
            where TCustomEntity : class;

        IQueryable<TCustomEntity> DbSet<TCustomEntity>(bool noTracking, bool ignoreQueryFilters = false)
            where TCustomEntity : class;

        IQueryable<TCustomEntity> Query<TCustomEntity>() where TCustomEntity : class;
        SoftDeleteAttribute SoftDeleteAttributeInstance<TCustomEntity>();
        bool HasSoftDeleteAttribute<TCustomEntity>();

        IValidator<RequestWithPagination<TCustomEntity>> GetPaginationValidator<TCustomEntity>()
            where TCustomEntity : class, new();

        IQueryable<TCustomEntity> GetAllAsQueryable<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
            where TCustomEntity : class;

        IList<TCustomEntity> GetAll<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
            where TCustomEntity : class;

        Task<IList<TCustomEntity>> GetAllAsync<TCustomEntity>(bool noTracking = true, bool ignoreQueryFilters = false)
            where TCustomEntity : class;

        public IOrderedQueryable<TCustomEntity> ListAsQueryable<TCustomEntity>(RequestWithPagination<TCustomEntity> request) where TCustomEntity : class, new();
        PaginationWrapper<TCustomEntity> List<TCustomEntity>(RequestWithPagination<TCustomEntity> request) where TCustomEntity : class, new();
        Task<PaginationWrapper<TCustomEntity>> ListAsync<TCustomEntity>(RequestWithPagination<TCustomEntity> request) where TCustomEntity : class, new();
        IQueryable<TCustomEntity> GetAsQueryable<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false) where TCustomEntity : class, new();

        IQueryable<TCustomEntity> GetByIdAsQueryable<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new();

        TCustomEntity Get<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false) where TCustomEntity : class, new();
        Task<TCustomEntity> GetAsync<TCustomEntity>(TCustomEntity entity, bool noTracking = false, bool ignoreQueryFilters = false) where TCustomEntity : class, new();
        TCustomEntity GetById<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false) where TCustomEntity : class, new();

        Task<TCustomEntity> GetByIdAsync<TCustomEntity, TCustomKey>(TCustomKey primaryId, bool noTracking = false, bool ignoreQueryFilters = false)
            where TCustomEntity : class, new();

        TCustomEntity Save<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class;

        Task<TCustomEntity> SaveAsync<TCustomEntity>(TCustomEntity entity)
            where TCustomEntity : class;

        TCustomEntity Save<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new() where TCustomEntity : class;

        Task<TCustomEntity> SaveAsync<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new() where TCustomEntity : class;

        TCustomEntity Save<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
            where TCustomEntity : class;

        Task<TCustomEntity> SaveAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator, bool ignoreValidation = false)
            where TCustomEntity : class;

        ValidationResult Validate<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new();

        Task<ValidationResult> ValidateAsync<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new();

        ValidationResult Validate<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator);
        Task<ValidationResult> ValidateAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator);

        void ValidateAndThrow<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new();

        Task ValidateAndThrowAsync<TCustomEntity, TCustomValidator>(TCustomEntity entity)
            where TCustomValidator : IValidator<TCustomEntity>, new();

        void ValidateAndThrow<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator);
        Task ValidateAndThrowAsync<TCustomEntity>(TCustomEntity entity, IValidator<TCustomEntity> validator);

        object GetPrimaryId<TEntity>(TEntity entity)
            where TEntity : class;

        TKey GetPrimaryId<TEntity, TKey>(TEntity entity)
            where TEntity : class;

        void SetPrimaryId<TEntity, TKey>(TEntity entity, TKey value)
            where TEntity : class;

        TEntity ConvertToEntity<TEntity, TKey>(TKey primaryId)
            where TEntity : class, new();
    }
}