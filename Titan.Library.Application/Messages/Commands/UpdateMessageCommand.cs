using MediatR;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Messages;
using Titan.Library.Domain.Messages;
using Titan.Library.Domain.Messages.Events;

namespace Titan.Library.Application.Messages.Commands;

public class UpdateMessageCommand : ICommand<MessageDto>
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class UpdateMessageCommandValidator : ICommandValidator<UpdateMessageCommand, MessageDto>
{
    public Result Validate(UpdateMessageCommand command)
    {
        if (command.Id <= 0)
            return Result.Fail(ApplicationMessageKeys.MESSAGE_NOT_FOUND);

        if (string.IsNullOrWhiteSpace(command.Key))
            return Result.Fail(ApplicationMessageKeys.MESSAGE_KEY_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Value))
            return Result.Fail(ApplicationMessageKeys.MESSAGE_VALUE_REQUIRED);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class UpdateMessageCommandHandler : BaseCommandHandler<UpdateMessageCommand, MessageDto>
{
    public override ICommandValidator<UpdateMessageCommand, MessageDto> Validator { get; set; } =
        new UpdateMessageCommandValidator();

    private readonly IMessageRepository _messageRepository;
    private readonly IPublisher _publisher;

    public UpdateMessageCommandHandler(IMessageRepository messageRepository, IPublisher publisher)
    {
        _messageRepository = messageRepository;
        _publisher = publisher;
    }

    protected override async Task<Result<MessageDto>> InnerHandle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.FindById(request.Id);
        if (message is null)
            return Result<MessageDto>.Fail(ApplicationMessageKeys.MESSAGE_NOT_FOUND);

        message.Key = request.Key;
        message.Value = request.Value;

        await _messageRepository.Update(message);

        _ = _publisher.Publish(new MessageUpdatedEvent { Key = message.Key }, CancellationToken.None);

        var dto = new MessageDto();
        dto.Map(message);

        return Result<MessageDto>.Success(dto, ApplicationMessageKeys.MESSAGE_UPDATED_SUCCESSFULLY);
    }
}
