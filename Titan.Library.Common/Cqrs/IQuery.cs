using MediatR;
using Titan.Library.Common.Results;

namespace Titan.Library.Common.Cqrs;

public interface IQuery<T> : IRequest<Result<T>>
{
}

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, Result<TResult>>
    where TQuery : IQuery<TResult>
{
}