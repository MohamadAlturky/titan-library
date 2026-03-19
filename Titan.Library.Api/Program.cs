using Scalar.AspNetCore;
using Serilog;
using Titan.Library.Api.Infrastructure;
using Titan.Library.Application;
using Titan.Library.Common.EndPoints;
using Titan.Library.Endpoints;
using Titan.Library.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration).AddApplication(builder.Configuration);

var app = builder.Build();

app.MapGet("/version", () => "1.0.1");
app.MapEndpoints(typeof(EndpointsAssemblyReference).Assembly);

app.UseHttpsRedirection();
app.MapOpenApi();
app.MapScalarApiReference();

// await app.UseSqlMigrations();
app.Run();
