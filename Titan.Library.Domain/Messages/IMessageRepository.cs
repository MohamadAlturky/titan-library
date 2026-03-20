using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Messages;

public interface IMessageRepository : IBaseRepository<Message, int>
{
    Task<(List<Message> items, int total)> GetPaginated(string? search, int page, int pageSize);
    Task<Message?> FindByKey(string key);
    Task<List<Message>> GetByKeys(IEnumerable<string> keys);
    Task<List<Message>> GetNotInKeys(IEnumerable<string> keys);
    Task InsertMany(IEnumerable<Message> messages);
    Task DeleteMany(IEnumerable<Message> messages);
}
