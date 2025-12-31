using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MyFinance.Common.Application;
using MyFinance.Pluggy.Application.Contracts.V1;
using MyFinance.Pluggy.Application.Services;

namespace MyFinance.Pluggy.Application.Connections;

internal sealed class CreateConnectToken : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/connect-token", Handle)
            .WithName("CreateConnectToken")
            .WithSummary("Create a Pluggy Connect Token")
            .WithDescription("Creates a temporary token to initialize the Pluggy Connect Widget")
            .Produces<CreateConnectTokenResponse>()
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> Handle(
        IPluggyConnectionService connectionService,
        ILogger<CreateConnectToken> logger,
        CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Creating Pluggy connect token");

            var accessToken = await connectionService.CreateConnectTokenAsync(ct);

            return TypedResults.Ok(new CreateConnectTokenResponse
            {
                AccessToken = accessToken
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Pluggy connect token");
            return TypedResults.Problem(
                title: "Failed to create connect token",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
