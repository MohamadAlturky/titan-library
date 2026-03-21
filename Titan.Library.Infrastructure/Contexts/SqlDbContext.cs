using System.Data;
using System.Data.Common;
using Titan.Library.Common.Storage;
using Titan.Library.Infrastructure.Connectors;

namespace Titan.Library.Infrastructure.Contexts;

public class SqlDbContext : ISqlDbContext
{
    private readonly IDbConnectionFactory _connectionFactory;
    private DbConnection? _connection;
    private DbTransaction? _transaction;
    private bool _disposed;

    public SqlDbContext(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DbConnection> GetOpenConnectionAsync(CancellationToken ct = default)
    {
        if (_connection == null)
        {
            _connection = _connectionFactory.CreateDbConnection();
        }

        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(ct);
        }

        return _connection;
    }

    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default
    )
    {
        if (_transaction != null)
            return;

        var conn = await GetOpenConnectionAsync(ct);
        _transaction = await conn.BeginTransactionAsync(isolationLevel, ct);
        return;
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<DbCommand> CreateCommandAsync()
    {
        var connection = await GetOpenConnectionAsync();
        var cmd = connection.CreateCommand();
        if (_transaction != null)
        {
            cmd.Transaction = _transaction;
        }

        return cmd;
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _transaction?.Dispose();
        _connection?.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        if (_transaction != null)
            await _transaction.DisposeAsync();
        if (_connection != null)
            await _connection.DisposeAsync();
        _disposed = true;
    }
}
