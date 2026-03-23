using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Messages;
using Titan.Library.Domain.Messages;

namespace Titan.Library.Application.Messages.Commands;

public class CreateMessageCommand : ICommand<MessageDto>
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class CreateMessageCommandValidator : ICommandValidator<CreateMessageCommand, MessageDto>
{
    public Result Validate(CreateMessageCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Key))
            return Result.Fail(ApplicationMessageKeys.MESSAGE_KEY_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Value))
            return Result.Fail(ApplicationMessageKeys.MESSAGE_VALUE_REQUIRED);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class CreateMessageCommandHandler : BaseCommandHandler<CreateMessageCommand, MessageDto>
{
    public override ICommandValidator<CreateMessageCommand, MessageDto> Validator { get; set; } =
        new CreateMessageCommandValidator();

    private readonly IMessageRepository _messageRepository;

    public CreateMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    protected override async Task<Result<MessageDto>> InnerHandle(
        CreateMessageCommand request,
        CancellationToken cancellationToken
    )
    {
        var message = Message.Create(request.Key, request.Value);
        var id = await _messageRepository.Add(message);
        message.Id = id;

        var dto = MessageDto.FromEntity(message);

        return Result<MessageDto>.Success(dto, ApplicationMessageKeys.MESSAGE_CREATED_SUCCESSFULLY);
    }
}
