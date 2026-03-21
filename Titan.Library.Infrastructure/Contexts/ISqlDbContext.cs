using System.Data;
using System.Data.Common;
using Titan.Library.Common.Storage;

namespace Titan.Library.Infrastructure.Contexts;

public interface ISqlDbContext : IDisposable, IAsyncDisposable, IAsyncUnitOfWork
{
    Task<DbConnection> GetOpenConnectionAsync(CancellationToken ct = default);
    Task<DbCommand> CreateCommandAsync();
    // Task<DbTransaction> BeginTransactionAsync(
    //     IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
    //     CancellationToken ct = default
    // );
}
