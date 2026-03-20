using MediatR;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Domain.Messages;
using Titan.Library.Domain.Messages.Events;

namespace Titan.Library.Application.Messages.Commands;

public class DeleteMessageCommand : ICommand<bool>
{
    public int Id { get; set; }
}

public class DeleteMessageCommandValidator : ICommandValidator<DeleteMessageCommand, bool>
{
    public Result Validate(DeleteMessageCommand command)
    {
        if (command.Id <= 0)
            return Result.Fail(ApplicationMessageKeys.MESSAGE_NOT_FOUND);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class DeleteMessageCommandHandler : BaseCommandHandler<DeleteMessageCommand, bool>
{
    public override ICommandValidator<DeleteMessageCommand, bool> Validator { get; set; } =
        new DeleteMessageCommandValidator();

    private readonly IMessageRepository _messageRepository;
    private readonly IPublisher _publisher;

    public DeleteMessageCommandHandler(IMessageRepository messageRepository, IPublisher publisher)
    {
        _messageRepository = messageRepository;
        _publisher = publisher;
    }

    protected override async Task<Result<bool>> InnerHandle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.FindById(request.Id);
        if (message is null)
            return Result<bool>.Fail(ApplicationMessageKeys.MESSAGE_NOT_FOUND);

        await _messageRepository.Delete(message);

        _ = _publisher.Publish(new MessageDeletedEvent { Key = message.Key }, CancellationToken.None);

        return Result<bool>.Success(true, ApplicationMessageKeys.MESSAGE_DELETED_SUCCESSFULLY);
    }
}
