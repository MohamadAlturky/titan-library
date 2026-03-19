using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Titan.Library.Common.EndPoints;

namespace Titan.Library.Common.EndPoints;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(
        this WebApplication app,
        EndpointGroupBase group,
        string route = "",
        string groupName = ""
    )
    {
        groupName = string.IsNullOrEmpty(groupName) ? group.GetType().Name : groupName;
        route = string.IsNullOrEmpty(route) ? groupName : route;

        return app.MapGroup($"/api/{route}").WithTags(groupName).WithOpenApi();
    }

    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointGroupType = typeof(EndpointGroupBase);

        var endpointGroupTypes = assembly
            .GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }
}