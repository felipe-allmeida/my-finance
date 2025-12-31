using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MyFinance.Common.Application;
using MyFinance.Pluggy.Application.Contracts.Pluggy;
using MyFinance.Pluggy.Application.Services;

namespace MyFinance.Pluggy.Application.Webhooks;

internal sealed class PluggyWebhook : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/webhook", Handle)
            .WithName("PluggyWebhook")
            .WithSummary("Receive Pluggy webhook events")
            .WithDescription("Endpoint to receive webhook notifications from Pluggy")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Handle(
        [FromBody] PluggyWebhookRequest request,
        [FromServices] ITransactionSyncService syncService,
        [FromServices] MyFinance.Common.Domain.IUnitOfWork uow,
        [FromServices] ILogger<PluggyWebhook> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Received Pluggy webhook event: {Event} for itemId: {ItemId}",
            request.Event, request.Data.ItemId);

        try
        {
            // Handle different webhook events
            switch (request.Event)
            {
                case "item/updated":
                case "item/created":
                    // Get userId from stored connection
                    var connection = uow.ReadSet<MyFinance.Pluggy.Domain.Connections.UserConnection>()
                        .FirstOrDefault(c => c.ItemId == request.Data.ItemId);

                    if (connection == null)
                    {
                        logger.LogWarning("No connection found for itemId: {ItemId}", request.Data.ItemId);
                        return TypedResults.BadRequest(new { error = "Connection not found" });
                    }

                    await syncService.SyncTransactionsAsync(
                        request.Data.ItemId,
                        connection.UserId,
                        ct);
                    break;

                case "item/error":
                    logger.LogWarning("Item error received for itemId: {ItemId}, error: {Error}",
                        request.Data.ItemId, request.Data.Error);
                    break;

                case "item/deleted":
                    logger.LogInformation("Item deleted: {ItemId}", request.Data.ItemId);
                    break;

                default:
                    logger.LogWarning("Unknown webhook event: {Event}", request.Event);
                    break;
            }

            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Pluggy webhook for itemId: {ItemId}", request.Data.ItemId);
            return TypedResults.BadRequest(new { error = "Failed to process webhook" });
        }
    }
}
