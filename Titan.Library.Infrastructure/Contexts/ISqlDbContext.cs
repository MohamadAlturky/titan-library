using System.Data;
using System.Data.Common;

namespace Titan.Library.Infrastructure.Contexts;

public interface ISqlDbContext : IDisposable, IAsyncDisposable
{
    Task<DbConnection> GetOpenConnectionAsync(CancellationToken ct = default);
    Task<DbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);

    Task<DbCommand> CreateCommandAsync();
}