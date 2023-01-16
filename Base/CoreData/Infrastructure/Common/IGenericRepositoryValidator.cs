using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace CoreData.Infrastructure
{
    public interface IGenericRepositoryValidator<TEntity, TKey, TValidator> : IGenericRepositoryValidator<TEntity, TValidator>, IGenericRepository<TEntity, TKey>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
    }

    public interface IGenericRepositoryValidator<TEntity, TValidator> : IGenericRepository<TEntity>, IGenericRepositoryValidator
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        TEntity Save(TEntity entity, bool ignoreValidation = false);
        Task<TEntity> SaveAsync(TEntity entity, bool ignoreValidation = false);
        ValidationResult Validate(TEntity entity);
        Task<ValidationResult> ValidateAsync(TEntity entity);
        void ValidateAndThrow(TEntity entity);
        Task ValidateAndThrowAsync(TEntity entity);
    }

    public interface IGenericRepositoryValidator
    {
    }
}