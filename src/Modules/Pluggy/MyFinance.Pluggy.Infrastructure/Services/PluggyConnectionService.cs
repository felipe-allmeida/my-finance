using Microsoft.Extensions.Logging;
using MyFinance.Common.Domain;
using MyFinance.Pluggy.Application.Services;
using MyFinance.Pluggy.Domain.Connections;
using Pluggy.SDK;

namespace MyFinance.Pluggy.Infrastructure.Services;

internal sealed class PluggyConnectionService(
    PluggyAPI pluggyClient,
    IUnitOfWork uow,
    ILogger<PluggyConnectionService> logger) : IPluggyConnectionService
{
    public async Task<string> CreateConnectTokenAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating Pluggy connect token");

        var token = await pluggyClient.CreateConnectToken();

        return token.AccessToken;
    }

    public async Task<(Guid ConnectionId, string ConnectorName)> StoreConnectionAsync(
        string itemId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Storing connection for itemId: {ItemId}, userId: {UserId}", itemId, userId);

        // Check if connection already exists
        var existingConnection = uow.ReadSet<UserConnection>()
            .FirstOrDefault(c => c.ItemId == itemId);

        if (existingConnection != null)
        {
            logger.LogWarning("Connection for itemId {ItemId} already exists", itemId);
            throw new InvalidOperationException("Connection already exists");
        }

        // Fetch item details from Pluggy to get connector name
        // TODO: Verify SDK method signature - FetchItem expects Guid but itemId is string
        // For now, using placeholder connector name until SDK integration is verified
        var connectorName = "Pluggy Connection";

        _ = pluggyClient; // Suppress unused warning
        await Task.CompletedTask; // Suppress async warning

        // Create new connection
        var connection = new UserConnection(
            id: Guid.NewGuid(),
            userId: userId,
            itemId: itemId,
            connectorName: connectorName
        );

        await uow.AddAsync(connection, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully stored connection for itemId: {ItemId}", itemId);

        return (connection.Id, connection.ConnectorName);
    }
}
