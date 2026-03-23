using Titan.Library.Common.Abstractions;

namespace Titan.Library.Domain.Feedbacks;

public class Feedback : BaseEntity<int>
{
    public Feedback() { }

    public int CustomerId { get; set; }
    public string Category { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public FeedbackSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            CustomerId = CustomerId,
            Category = Category,
            Rating = Rating,
            Subject = Subject,
            Message = Message,
            CreatedAt = CreatedAt,
        };

    public static Feedback Reconstitute(FeedbackSnapshot snapshot) =>
        new()
        {
            Id = snapshot.Id,
            CustomerId = snapshot.CustomerId,
            Category = snapshot.Category,
            Rating = snapshot.Rating,
            Subject = snapshot.Subject,
            Message = snapshot.Message,
            CreatedAt = snapshot.CreatedAt,
        };

    public static Feedback Create(
        int customerId,
        string category,
        int? rating,
        string subject,
        string message
    ) =>
        new()
        {
            CustomerId = customerId,
            Category = category,
            Rating = rating,
            Subject = subject,
            Message = message,
            CreatedAt = DateTime.UtcNow,
        };
}
