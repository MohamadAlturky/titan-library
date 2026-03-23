using System.Data.Common;
using Titan.Library.Domain.Feedbacks;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;
using C = Titan.Library.Infrastructure.Configurations.FeedbackTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.FeedbackTableConfiguration;

namespace Titan.Library.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ISqlDbContext _dbContext;

    public FeedbackRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(Feedback entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            INSERT INTO {T.Table} ({C.CustomerId}, {C.Category}, {C.Rating}, {C.Subject}, {C.Message}, {C.CreatedAt})
            VALUES (@CustomerId, @Category, @Rating, @Subject, @Message, @CreatedAt)
            RETURNING {C.Id};
            """;

        command.AddParameters(
            new
            {
                entity.CustomerId,
                entity.Category,
                entity.Rating,
                entity.Subject,
                entity.Message,
                entity.CreatedAt,
            }
        );

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task<IEnumerable<Feedback>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.CustomerId}, {C.Category}, {C.Rating}, {C.Subject}, {C.Message}, {C.CreatedAt}
            FROM {T.Table}
            ORDER BY {C.CreatedAt} DESC;
            """;

        return await command.ExecuteListAsync(MapToFeedback);
    }

    public async Task<IEnumerable<Feedback>> FindByCustomerId(int customerId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.CustomerId}, {C.Category}, {C.Rating}, {C.Subject}, {C.Message}, {C.CreatedAt}
            FROM {T.Table}
            WHERE {C.CustomerId} = @CustomerId
            ORDER BY {C.CreatedAt} DESC;
            """;

        command.AddParameters(new { CustomerId = customerId });

        return await command.ExecuteListAsync(MapToFeedback);
    }

    private static Feedback MapToFeedback(DbDataReader reader)
    {
        var ratingOrdinal = reader.GetOrdinal(C.Rating);
        var snapshot = new FeedbackSnapshot
        {
            Id = reader.GetInt32(reader.GetOrdinal(C.Id)),
            CustomerId = reader.GetInt32(reader.GetOrdinal(C.CustomerId)),
            Category = reader.GetString(reader.GetOrdinal(C.Category)),
            Rating = reader.IsDBNull(ratingOrdinal) ? null : reader.GetInt32(ratingOrdinal),
            Subject = reader.GetString(reader.GetOrdinal(C.Subject)),
            Message = reader.GetString(reader.GetOrdinal(C.Message)),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt)),
        };
        return Feedback.Reconstitute(snapshot);
    }
}
