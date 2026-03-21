using Scalar.AspNetCore;
using Serilog;
using Titan.Library.Api.Infrastructure;
using Titan.Library.Api.Infrastructure.Logging;
using Titan.Library.Api.Infrastructure.Middleware;
using Titan.Library.Application;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Logging;
using Titan.Library.Endpoints;
using Titan.Library.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration).AddApplication(builder.Configuration);
builder.Services.AddScoped<IApiResponseResolver, ApiResponseResolver>();
builder.Services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
builder.Services.AddScoped(typeof(ITitanLogger<>), typeof(TitanLogger<>));

var app = builder.Build();
var context = app.Services.GetRequiredService<IHttpContextAccessor>();
AppHttpContext.Configure(context);

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapGet("/version", () => "1.0.1");
app.MapEndpoints(typeof(EndpointsAssemblyReference).Assembly);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference();

await app.UseSqlMigrations();
await app.UseMessageKeysSeeder();
app.Run();
