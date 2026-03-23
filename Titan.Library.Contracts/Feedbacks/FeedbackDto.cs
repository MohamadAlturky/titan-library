using Titan.Library.Domain.Feedbacks;

namespace Titan.Library.Contracts.Feedbacks;

public class FeedbackDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Category { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public static FeedbackDto FromEntity(Feedback entity) =>
        new()
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Category = entity.Category,
            Rating = entity.Rating,
            Subject = entity.Subject,
            Message = entity.Message,
            CreatedAt = entity.CreatedAt,
        };
}
