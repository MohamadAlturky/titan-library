using Titan.Library.Common.Abstractions;

namespace Titan.Library.Common.Dtos;

public abstract class BaseDto<T, TKey>
    where T : BaseEntity<TKey>
{
    public abstract void Map(T entity);
}