namespace Titan.Library.Domain.Feedbacks;

public class FeedbackSnapshot
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Category { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
