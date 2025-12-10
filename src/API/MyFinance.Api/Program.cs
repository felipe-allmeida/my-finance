using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyFinance.Api.Extensions;
using MyFinance.Common.Infrastructure;
using MyFinance.Common.Infrastructure.Extensions;
using MyFinance.Ledger.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerDocumentation();

Assembly[] moduleApplicationAssemblies = [MyFinance.Ledger.Application.AssemblyReference.Assembly];

var databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");
var redisConnectionString = builder.Configuration.GetConnectionStringOrThrow("Cache");

builder.Services.AddInfrastructure("MyFinance", [], databaseConnectionString, redisConnectionString);

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(redisConnectionString);

builder.Services.AddLedgerModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // TODO:
    //app.UseSwagger();
    //app.UseSwaggerUI();

    //app.ApplyMigrations();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

app.Run();

public partial class Program;
