using System.Threading.Tasks;
using CoreData.Infrastructure;
using CoreType.Types;
using FluentValidation;

namespace CoreSvc.Services
{
    public interface IGenericServiceValidator<TEntity, TKey, TValidator, out TRepository> : IGenericServiceValidator<TEntity, TValidator, TRepository>, IGenericServiceBasic<TEntity, TKey, TRepository>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
    }

    public interface IGenericServiceValidator<TEntity, TValidator, out TRepository> : IGenericService<TEntity, TRepository>
        where TEntity : class, new()
        where TValidator : IValidator<TEntity>, new()
    {
        ResponseWrapper<TEntity> Save(TEntity entity, bool ignoreValidation = false);
        Task<ResponseWrapper<TEntity>> SaveAsync(TEntity entity, bool ignoreValidation = false);
    }
}