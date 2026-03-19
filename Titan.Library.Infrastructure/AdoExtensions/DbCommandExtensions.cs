using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Titan.Library.Infrastructure.AdoExtensions;

public static class DbCommandExtensions
{
    // --- Parameter Extensions ---

    private static DbCommand AddParameter(this DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);

        return command;
    }

    public static DbCommand AddParameters(this DbCommand command, IEnumerable<(string Name, object? Value)> parameters)
    {
        foreach (var (name, value) in parameters)
        {
            command.AddParameter(name, value);
        }

        return command;
    }

    /// <summary>
    /// Allows passing parameters as an anonymous object (e.g., .AddParameters(new { Id = 1, Name = "Titan" }))
    /// </summary>
    public static DbCommand AddParameters(this DbCommand command, object parameters)
    {
        if (parameters == null) return command;

        var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            command.AddParameter(prop.Name, prop.GetValue(parameters));
        }

        return command;
    }

    // --- Synchronous Execution Extensions ---

    public static List<T> ExecuteList<T>(this DbCommand command, Func<DbDataReader, T> map)
    {
        var list = new List<T>();

        if (command.Connection?.State == ConnectionState.Closed)
            command.Connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(map(reader));
        }

        return list;
    }

    public static T? ExecuteSingleOrDefault<T>(this DbCommand command, Func<DbDataReader, T> map)
    {
        if (command.Connection?.State == ConnectionState.Closed)
            command.Connection.Open();

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return map(reader);
        }

        return default;
    }

    public static T? ExecuteScalarValue<T>(this DbCommand command)
    {
        if (command.Connection?.State == ConnectionState.Closed)
            command.Connection.Open();

        var result = command.ExecuteScalar();
        if (result == null || result == DBNull.Value)
        {
            return default;
        }

        // ChangeType handles safe conversions (e.g., converting a SQL BIGINT to a C# int if needed)
        var underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(result, underlyingType);
    }

    public static int ExecuteNonQuerySafe(this DbCommand command)
    {
        if (command.Connection?.State == ConnectionState.Closed)
            command.Connection.Open();

        return command.ExecuteNonQuery();
    }

    // --- Asynchronous Execution Extensions ---

    public static async Task<List<T>> ExecuteListAsync<T>(this DbCommand command, Func<DbDataReader, T> map,
        CancellationToken cancellationToken = default)
    {
        var list = new List<T>();

        if (command.Connection?.State == ConnectionState.Closed)
            await command.Connection.OpenAsync(cancellationToken);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(map(reader));
        }

        return list;
    }

    public static async Task<T?> ExecuteSingleOrDefaultAsync<T>(this DbCommand command, Func<DbDataReader, T> map,
        CancellationToken cancellationToken = default)
    {
        if (command.Connection?.State == ConnectionState.Closed)
            await command.Connection.OpenAsync(cancellationToken);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return map(reader);
        }

        return default;
    }

    public static async Task<T?> ExecuteScalarValueAsync<T>(this DbCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Connection?.State == ConnectionState.Closed)
            await command.Connection.OpenAsync(cancellationToken);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (result == null || result == DBNull.Value)
        {
            return default;
        }

        var underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(result, underlyingType);
    }

    public static async Task<int> ExecuteNonQuerySafeAsync(this DbCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Connection?.State == ConnectionState.Closed)
            await command.Connection.OpenAsync(cancellationToken);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }
}