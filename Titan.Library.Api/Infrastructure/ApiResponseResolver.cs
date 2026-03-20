using MediatR;
using Titan.Library.Application.Messages.Queries;
using Titan.Library.Common.EndPoints;

namespace Titan.Library.Api.Infrastructure;

public class ApiResponseResolver : IApiResponseResolver
{
    private readonly ISender _sender;

    public ApiResponseResolver(ISender sender)
    {
        _sender = sender;
    }

    public async Task<string> ResolveAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _sender.Send(new GetMessageByKeyQuery { Key = key }, cancellationToken);
        return result.IsSuccess && result.Data is not null ? result.Data.Value : key;
    }
}
