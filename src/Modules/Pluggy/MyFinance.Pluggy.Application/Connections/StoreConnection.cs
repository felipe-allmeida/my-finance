using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MyFinance.Common.Application;
using MyFinance.Pluggy.Application.Contracts.V1;
using MyFinance.Pluggy.Application.Services;

namespace MyFinance.Pluggy.Application.Connections;

internal sealed class StoreConnection : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/connections", Handle)
            .WithName("StoreConnection")
            .WithSummary("Store a Pluggy connection")
            .WithDescription("Stores the mapping between a Pluggy itemId and a user")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> Handle(
        [FromBody] StoreConnectionRequest request,
        [FromServices] IPluggyConnectionService connectionService,
        [FromServices] ILogger<StoreConnection> logger,
        CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Storing connection for itemId: {ItemId}, userId: {UserId}",
                request.ItemId, request.UserId);

            var (connectionId, connectorName) = await connectionService.StoreConnectionAsync(
                request.ItemId,
                request.UserId,
                ct);

            logger.LogInformation("Successfully stored connection for itemId: {ItemId}", request.ItemId);

            return TypedResults.Created($"/api/pluggy/connections/{connectionId}",
                new { id = connectionId, connectorName });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            logger.LogWarning("Connection for itemId {ItemId} already exists", request.ItemId);
            return TypedResults.BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to store connection for itemId: {ItemId}", request.ItemId);
            return TypedResults.Problem(
                title: "Failed to store connection",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
