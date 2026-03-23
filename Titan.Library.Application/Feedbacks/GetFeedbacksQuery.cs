using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Feedbacks;
using Titan.Library.Domain.Feedbacks;

namespace Titan.Library.Application.Feedbacks;

public class GetFeedbacksQuery : IQuery<List<FeedbackDto>> { }

public class GetFeedbacksQueryHandler : IQueryHandler<GetFeedbacksQuery, List<FeedbackDto>>
{
    private readonly IFeedbackRepository _feedbackRepository;

    public GetFeedbacksQueryHandler(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public async Task<Result<List<FeedbackDto>>> Handle(
        GetFeedbacksQuery request,
        CancellationToken cancellationToken
    )
    {
        var feedbacks = await _feedbackRepository.ToList();
        var result = feedbacks.Select(FeedbackDto.FromEntity).ToList();

        return Result<List<FeedbackDto>>.Success(
            result,
            ApplicationMessageKeys.FEEDBACKS_RETRIEVED_SUCCESSFULLY
        );
    }
}
