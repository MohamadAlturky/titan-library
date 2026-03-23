using Scalar.AspNetCore;
using Serilog;
using Titan.Library.Api.Infrastructure;
using Titan.Library.Api.Infrastructure.Logging;
using Titan.Library.Api.Infrastructure.Middleware;
using Titan.Library.Api.Infrastructure.Observability;
using Titan.Library.Application;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Logging;
using Titan.Library.Endpoints;
using Titan.Library.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (ctx, config) =>
        config.ReadFrom.Configuration(ctx.Configuration).Enrich.With<ActivityEnricher>()
);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
);
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddObservability(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration).AddApplication(builder.Configuration);
builder.Services.AddScoped<IApiResponseResolver, ApiResponseResolver>();
builder.Services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
builder.Services.AddScoped(typeof(ITitanLogger<>), typeof(TitanLogger<>));
builder.Services.AddSingleton(typeof(CorrelationIdMiddleware));
builder.Services.AddSingleton(typeof(ExceptionHandlerMiddleware));
var app = builder.Build();
var context = app.Services.GetRequiredService<IHttpContextAccessor>();
AppHttpContext.Configure(context);

app.UseCors();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapPrometheusScrapingEndpoint("/metrics");
app.MapGet("/version", () => "1.0.1");
app.MapEndpoints(typeof(EndpointsAssemblyReference).Assembly);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference();


await app.UseSqlMigrations();
await app.UseMessageKeysSeeder();
await app.UseAdminUserSeeder();

app.Run();
