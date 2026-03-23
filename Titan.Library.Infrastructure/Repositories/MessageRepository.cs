using System.Data.Common;
using Npgsql;
using NpgsqlTypes;
using Titan.Library.Domain.Messages;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Configurations;
using Titan.Library.Infrastructure.Contexts;
using C = Titan.Library.Infrastructure.Configurations.MessageTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.MessageTableConfiguration;

namespace Titan.Library.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ISqlDbContext _dbContext;

    public MessageRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(Message entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            INSERT INTO {T.Table} ({C.Key}, {C.Value})
            VALUES (@Key, @Value)
            RETURNING {C.Id};
            """;

        command.AddParameters(new { entity.Key, entity.Value });

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task Update(Message entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.Key} = @Key, {C.Value} = @Value
            WHERE {C.Id} = @Id;
            """;

        command.AddParameters(
            new
            {
                entity.Id,
                entity.Key,
                entity.Value,
            }
        );

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task Delete(Message entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"DELETE FROM {T.Table} WHERE {C.Id} = @Id;";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Message>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"SELECT {C.Id}, {C.Key}, {C.Value}, {C.CreatedAt} FROM {T.Table};";

        return await command.ExecuteListAsync(MapToMessage);
    }

    public async Task<Message?> FindByKey(string key)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Key}, {C.Value}, {C.CreatedAt} FROM {T.Table} WHERE {C.Key} = @Key LIMIT 1;";

        command.AddParameters(new { Key = key });

        return await command.ExecuteSingleOrDefaultAsync(MapToMessage);
    }

    public async Task<Message?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Key}, {C.Value}, {C.CreatedAt} FROM {T.Table} WHERE {C.Id} = @Id;";

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToMessage);
    }

    public async Task<(List<Message> items, int total)> GetPaginated(
        string? search,
        string orderBy,
        bool ascending,
        int page,
        int pageSize
    )
    {
        await using var command = await _dbContext.CreateCommandAsync();

        var offset = (page - 1) * pageSize;
        var searchParam = search is not null ? $"%{search}%" : null;
        var allowedSortColumns = new HashSet<string> { "id", "key", "value", "created_at" };
        var sortColumn = allowedSortColumns.Contains(orderBy) ? orderBy : "id";
        var direction = ascending ? "ASC" : "DESC";

        command.CommandText = $"""
            SELECT {C.Id}, {C.Key}, {C.Value}, {C.CreatedAt}, COUNT(*) OVER() AS total_count
            FROM {T.Table}
            WHERE (@Search IS NULL OR {C.Key} ILIKE @Search OR {C.Value} ILIKE @Search)
            ORDER BY {sortColumn} {direction}
            LIMIT @PageSize OFFSET @Offset;
            """;

        command.Parameters.Add(
            new NpgsqlParameter("Search", NpgsqlDbType.Text)
            {
                Value = searchParam ?? (object)DBNull.Value,
            }
        );
        command.AddParameters(new { PageSize = pageSize, Offset = offset });

        var total = 0;
        var items = await command.ExecuteListAsync(reader =>
        {
            total = (int)reader.GetInt64(reader.GetOrdinal("total_count"));
            return MapToMessage(reader);
        });

        return (items.ToList(), total);
    }

    public async Task<List<Message>> GetByKeys(IEnumerable<string> keys)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Key}, {C.Value}, {C.CreatedAt} FROM {T.Table} WHERE {C.Key} = ANY(@Keys);";

        command.Parameters.Add(
            new NpgsqlParameter("Keys", NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = keys.ToArray(),
            }
        );

        return (await command.ExecuteListAsync(MapToMessage)).ToList();
    }

    public async Task<List<Message>> GetNotInKeys(IEnumerable<string> keys)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Key}, {C.Value}, {C.CreatedAt} FROM {T.Table} WHERE {C.Key} != ALL(@Keys);";

        command.Parameters.Add(
            new NpgsqlParameter("Keys", NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = keys.ToArray(),
            }
        );

        return (await command.ExecuteListAsync(MapToMessage)).ToList();
    }

    public async Task InsertMany(IEnumerable<Message> messages)
    {
        var list = messages.ToList();
        if (list.Count == 0)
            return;

        await using var command = await _dbContext.CreateCommandAsync();

        var valueParts = new List<string>();
        for (var i = 0; i < list.Count; i++)
        {
            valueParts.Add($"(@Key{i}, @Value{i})");
            command.Parameters.Add(new NpgsqlParameter($"Key{i}", list[i].Key));
            command.Parameters.Add(new NpgsqlParameter($"Value{i}", list[i].Value));
        }

        command.CommandText =
            $"INSERT INTO {T.Table} ({C.Key}, {C.Value}) VALUES {string.Join(", ", valueParts)};";

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task DeleteMany(IEnumerable<Message> messages)
    {
        var ids = messages.Select(m => m.Id).ToArray();
        if (ids.Length == 0)
            return;

        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"DELETE FROM {T.Table} WHERE {C.Id} = ANY(@Ids);";

        command.Parameters.Add(
            new NpgsqlParameter("Ids", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = ids }
        );

        await command.ExecuteNonQuerySafeAsync();
    }

    private static Message MapToMessage(DbDataReader reader)
    {
        int id = reader.GetInt32(reader.GetOrdinal(C.Id));
        string key = reader.GetString(reader.GetOrdinal(C.Key));
        string value = reader.GetString(reader.GetOrdinal(C.Value));
        DateTime createdAt = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt));
        return Message.Map(id, key, value, createdAt);
    }
}
