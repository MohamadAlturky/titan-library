using System.Reflection;
using MediatR;
using Titan.Library.Domain.Messages;
using Titan.Library.Domain.Messages.Events;

namespace Titan.Library.Application.Messages.Services;

public class ApplicationMessageKeysDataSeeder
{
    private readonly IMessageRepository _messageRepository;
    private readonly IPublisher _publisher;

    public ApplicationMessageKeysDataSeeder(
        IMessageRepository messageRepository,
        IPublisher publisher
    )
    {
        _messageRepository = messageRepository;
        _publisher = publisher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var allKeys = typeof(ApplicationMessageKeys)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();

        var existingMessages = await _messageRepository.GetByKeys(allKeys);
        var existingKeys = existingMessages.Select(m => m.Key).ToHashSet();

        var newMessages = allKeys
            .Where(k => !existingKeys.Contains(k))
            .Select(k => Message.Create(k, k))
            .ToList();

        var obsoleteMessages = await _messageRepository.GetNotInKeys(allKeys);

        if (newMessages.Count > 0)
            await _messageRepository.InsertMany(newMessages);

        if (obsoleteMessages.Count > 0)
        {
            await _messageRepository.DeleteMany(obsoleteMessages);
            await _publisher.Publish(
                new MessagesManyDeletedEvent { Keys = obsoleteMessages.Select(m => m.Key) },
                cancellationToken
            );
        }
    }
}
