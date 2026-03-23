namespace Titan.Library.Domain.Feedbacks;

public interface IFeedbackRepository
{
    Task<int> Add(Feedback entity);
    Task<IEnumerable<Feedback>> ToList();
    Task<IEnumerable<Feedback>> FindByCustomerId(int customerId);
}
