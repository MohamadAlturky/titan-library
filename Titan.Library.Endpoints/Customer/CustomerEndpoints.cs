using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Customers;
using Titan.Library.Common.EndPoints;
using Titan.Library.Contracts.Users;

namespace Titan.Library.Endpoints.Customer;

public class CustomerEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "Customers", "Customers");

        group
            .MapPost("/", CreateCustomerAsync)
            .WithName(nameof(CreateCustomerAsync))
            .WithSummary("Create a new Customer")
            .Produces(StatusCodes.Status200OK, typeof(CustomerDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/", GetCustomersAsync)
            .WithName(nameof(GetCustomersAsync))
            .WithSummary("Get a list of Customers")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<CustomerDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/{id}", GetCustomerByIdAsync)
            .WithName(nameof(GetCustomerByIdAsync))
            .WithSummary("Get a Customer by Id")
            .Produces(StatusCodes.Status200OK, typeof(CustomerDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapDelete("/{id}", DeleteCustomerAsync)
            .WithName(nameof(DeleteCustomerAsync))
            .WithSummary("Delete a Customer")
            .Produces(StatusCodes.Status200OK, typeof(bool))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));
    }

    private static async Task<IResult> CreateCustomerAsync(
        [FromServices] ISender sender,
        [FromBody] CreateCustomerCommand request
    )
    {
        var result = await sender.Send(request);
        return HandleApiResponse(result);
    }

    private static async Task<IResult> GetCustomersAsync(
        [FromServices] ISender sender,
        [AsParameters] GetCustomersQuery query
    )
    {
        var result = await sender.Send(query);
        return HandleApiResponse(result);
    }

    private static async Task<IResult> GetCustomerByIdAsync(
        [FromServices] ISender sender,
        [FromRoute] int id
    )
    {
        var result = await sender.Send(new GetCustomerByIdQuery { Id = id });
        return HandleApiResponse(result);
    }

    private static async Task<IResult> DeleteCustomerAsync(
        [FromServices] ISender sender,
        [FromRoute] int id
    )
    {
        var result = await sender.Send(new DeleteCustomerCommand { Id = id });
        return HandleApiResponse(result);
    }
}
