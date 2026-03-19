using MediatR;
using Titan.Library.Common.Results;

namespace Titan.Library.Common.Cqrs;

public interface ICommand<T> : IRequest<Result<T>>
{
}

public interface ICommandHandler<in T, TResult> : IRequestHandler<T, Result<TResult>>
    where T : ICommand<TResult>
{
}

public interface ICommandValidator<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Result Validate(TCommand command);
}

public abstract class BaseCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public abstract ICommandValidator<TCommand, TResult> Validator { get; set; }

    public async Task<Result<TResult>> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var validationResult = Validator.Validate(request);
        if (!validationResult.IsSuccess)
        {
            return Result<TResult>.Fail(validationResult.MessageCode);
        }

        return await InnerHandle(request, cancellationToken);
    }

    protected abstract Task<Result<TResult>> InnerHandle(TCommand request, CancellationToken cancellationToken);
}
