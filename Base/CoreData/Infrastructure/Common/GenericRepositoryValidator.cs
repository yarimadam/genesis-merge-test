using System.Threading.Tasks;
using CoreType.Types;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CoreData.Infrastructure
{
    public abstract class GenericRepositoryValidator<TEntity, TKey, TValidator> : GenericRepositoryBase<TEntity, TKey>, IGenericRepositoryValidator<TEntity, TKey, TValidator>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        protected GenericRepositoryValidator(DbContext context, SessionContext session = null) : base(context, session)
        {
        }

        #region Save

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ TEntity Save(TEntity entity, bool ignoreValidation = false)
        {
            return SaveAsync(entity, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<TEntity> SaveAsync(TEntity entity, bool ignoreValidation = false)
        {
            return await SaveAsync(entity, new TValidator(), ignoreValidation);
        }

        #endregion

        #endregion

        #region Validations

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ ValidationResult Validate(TEntity entity)
        {
            return ValidateAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ValidationResult> ValidateAsync(TEntity entity)
        {
            return await ValidateAsync(entity, new TValidator());
        }

        public /*virtual*/ void ValidateAndThrow(TEntity entity)
        {
            ValidateAndThrowAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task ValidateAndThrowAsync(TEntity entity)
        {
            await ValidateAndThrowAsync(entity, new TValidator());
        }

        #endregion

        #endregion
    }

    public abstract class GenericRepositoryValidator<TEntity, TValidator> : GenericRepositoryBase<TEntity>, IGenericRepositoryValidator<TEntity, TValidator>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        protected GenericRepositoryValidator(DbContext context, SessionContext session = null) : base(context, session)
        {
        }

        #region Save

        #region Overrides

        #endregion

        #region Extensions

        public TEntity Save(TEntity entity, bool ignoreValidation = false)
        {
            return SaveAsync(entity, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<TEntity> SaveAsync(TEntity entity, bool ignoreValidation = false)
        {
            return await SaveAsync(entity, new TValidator(), ignoreValidation);
        }

        #endregion

        #endregion

        #region Validations

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ ValidationResult Validate(TEntity entity)
        {
            return ValidateAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ValidationResult> ValidateAsync(TEntity entity)
        {
            return await ValidateAsync(entity, new TValidator());
        }

        public /*virtual*/ void ValidateAndThrow(TEntity entity)
        {
            ValidateAndThrowAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task ValidateAndThrowAsync(TEntity entity)
        {
            await ValidateAndThrowAsync(entity, new TValidator());
        }

        #endregion

        #endregion
    }
}