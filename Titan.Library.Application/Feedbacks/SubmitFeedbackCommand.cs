using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Feedbacks;
using Titan.Library.Domain.Feedbacks;

namespace Titan.Library.Application.Feedbacks;

public class SubmitFeedbackCommand : ICommand<FeedbackDto>
{
    public int CustomerId { get; set; }
    public string Category { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class SubmitFeedbackCommandValidator : ICommandValidator<SubmitFeedbackCommand, FeedbackDto>
{
    public Result Validate(SubmitFeedbackCommand command)
    {
        if (command.CustomerId <= 0)
            return Result.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND);

        if (string.IsNullOrWhiteSpace(command.Subject))
            return Result.Fail(ApplicationMessageKeys.FEEDBACK_SUBJECT_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Message))
            return Result.Fail(ApplicationMessageKeys.FEEDBACK_MESSAGE_REQUIRED);

        if (command.Rating.HasValue && (command.Rating < 1 || command.Rating > 5))
            return Result.Fail(ApplicationMessageKeys.FEEDBACK_INVALID_RATING);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class SubmitFeedbackCommandHandler : BaseCommandHandler<SubmitFeedbackCommand, FeedbackDto>
{
    public override ICommandValidator<SubmitFeedbackCommand, FeedbackDto> Validator { get; set; } =
        new SubmitFeedbackCommandValidator();

    private readonly IFeedbackRepository _feedbackRepository;

    public SubmitFeedbackCommandHandler(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    protected override async Task<Result<FeedbackDto>> InnerHandle(
        SubmitFeedbackCommand request,
        CancellationToken cancellationToken
    )
    {
        var feedback = Feedback.Create(
            request.CustomerId,
            request.Category,
            request.Rating,
            request.Subject,
            request.Message
        );

        feedback.Id = await _feedbackRepository.Add(feedback);

        return Result<FeedbackDto>.Success(
            FeedbackDto.FromEntity(feedback),
            ApplicationMessageKeys.FEEDBACK_SUBMITTED_SUCCESSFULLY
        );
    }
}
