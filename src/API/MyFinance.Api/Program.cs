using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyFinance.Api.Extensions;
using MyFinance.Common.Infrastructure;
using MyFinance.Common.Infrastructure.Extensions;
using MyFinance.Ledger.Infrastructure;
using MyFinance.Pluggy.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // Development fallback - allow all
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "MyFinance API",
        Version = "v1",
        Description = "API for MyFinance application"
    });
});

Assembly[] moduleApplicationAssemblies = [MyFinance.Ledger.Application.AssemblyReference.Assembly];

var databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");
var redisConnectionString = builder.Configuration.GetConnectionStringOrThrow("Cache");

builder.Services.AddInfrastructure("MyFinance", [], databaseConnectionString, redisConnectionString);

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(redisConnectionString);

builder.Services.AddLedgerModule(builder.Configuration);
builder.Services.AddPluggyModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseCors();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

// Map all endpoints under /api
var apiGroup = app.MapGroup("/api");

// Map ledger endpoints under /api/ledger
var ledgerGroup = apiGroup.MapGroup("/ledger");
foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<MyFinance.Common.Application.IEndpoint>>()
    .Where(e => e.GetType().Namespace?.Contains("Ledger") == true))
{
    endpoint.Map(ledgerGroup);
}

// Map pluggy endpoints under /api/pluggy
var pluggyGroup = apiGroup.MapGroup("/pluggy");
foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<MyFinance.Common.Application.IEndpoint>>()
    .Where(e => e.GetType().Namespace?.Contains("Pluggy") == true))
{
    endpoint.Map(pluggyGroup);
}

app.Run();

public partial class Program;
