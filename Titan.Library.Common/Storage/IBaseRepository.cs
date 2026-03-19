using Titan.Library.Common.Abstractions;

namespace Titan.Library.Common.Storage;

public interface IBaseRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    Task<TKey> Add(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
    Task<IEnumerable<TEntity>> ToList();
    Task<TEntity> FindById(TKey id);
}