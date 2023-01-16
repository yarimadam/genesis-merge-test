using System.Threading.Tasks;
using CoreData.Infrastructure;
using CoreType.Types;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CoreSvc.Services
{
    public abstract class GenericServiceValidator<TEntity, TKey, TValidator, TRepository> : GenericServiceBase<TEntity, TKey, TRepository>,
        IGenericServiceValidator<TEntity, TKey, TValidator, TRepository>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
        where TRepository : GenericRepositoryValidator<TEntity, TKey, TValidator>
    {
        protected GenericServiceValidator(DbContext context = null) : base(context)
        {
        }

        #region Save

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ ResponseWrapper<TEntity> Save(TEntity entity, bool ignoreValidation = false)
        {
            return SaveAsync(entity, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<TEntity>> SaveAsync(TEntity entity, bool ignoreValidation = false)
        {
            return await SaveAsync(entity, new TValidator(), ignoreValidation);
        }

        #endregion

        #endregion
    }

    public abstract class GenericServiceValidator<TEntity, TValidator, TRepository> : GenericServiceBase<TEntity, TRepository>, IGenericServiceValidator<TEntity, TValidator, TRepository>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
        where TRepository : GenericRepositoryValidator<TEntity, TValidator>
    {
        protected GenericServiceValidator(DbContext context = null) : base(context)
        {
        }

        #region Save

        #region Overrides

        #endregion

        #region Extensions

        public /*virtual*/ ResponseWrapper<TEntity> Save(TEntity entity, bool ignoreValidation = false)
        {
            return SaveAsync(entity, ignoreValidation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public /*virtual*/ async Task<ResponseWrapper<TEntity>> SaveAsync(TEntity entity, bool ignoreValidation = false)
        {
            return await SaveAsync(entity, new TValidator(), ignoreValidation);
        }

        #endregion

        #endregion
    }
}