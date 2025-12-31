using Microsoft.Extensions.Logging;
using MyFinance.Common.Domain;
using MyFinance.Ledger.Domain.Transactions;
using MyFinance.Pluggy.Application.Services;
using MyFinance.Pluggy.Domain.SyncHistory;
using Pluggy.SDK;

namespace MyFinance.Pluggy.Infrastructure.Services;

internal sealed class TransactionSyncService(
    PluggyAPI _pluggyClient,
    IUnitOfWork pluggyUnitOfWork,
    IUnitOfWork ledgerUnitOfWork,
    ILogger<TransactionSyncService> logger) : ITransactionSyncService
{
    public async Task SyncTransactionsAsync(string itemId, Guid userId, CancellationToken cancellationToken = default)
    {
        var syncHistory = new SyncHistory(
            Guid.NewGuid(),
            userId,
            itemId,
            SyncTrigger.Webhook);

        await pluggyUnitOfWork.AddAsync(syncHistory, cancellationToken);
        await pluggyUnitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            logger.LogInformation("Starting transaction sync for itemId: {ItemId}, userId: {UserId}", itemId, userId);

            // TODO: Implement actual Pluggy SDK transaction fetching
            // The Pluggy SDK method signature needs to be verified
            // For now, marking as successful with 0 transactions
            logger.LogWarning("Transaction sync not fully implemented - Pluggy SDK integration pending");

            // Placeholder to use _pluggyClient when SDK methods are verified
            _ = _pluggyClient;

            int fetched = 0;
            int created = 0;
            int failed = 0;

            // Placeholder for actual implementation:
            // 1. Fetch transactions using Pluggy SDK (e.g., await pluggyClient.FetchTransactions(itemId))
            // 2. Loop through transactions
            // 3. Check if transaction exists by ExternalId
            // 4. Map Pluggy transaction to our Transaction entity
            // 5. Save to database

            await ledgerUnitOfWork.SaveChangesAsync(cancellationToken);

            syncHistory.Complete(fetched, created, failed);
            pluggyUnitOfWork.Update(syncHistory);
            await pluggyUnitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Completed transaction sync for itemId: {ItemId}. Fetched: {Fetched}, Created: {Created}, Failed: {Failed}",
                itemId, fetched, created, failed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to sync transactions for itemId: {ItemId}", itemId);

            syncHistory.Complete(0, 0, 0, ex.Message);
            pluggyUnitOfWork.Update(syncHistory);
            await pluggyUnitOfWork.SaveChangesAsync(cancellationToken);

            throw;
        }
    }
}
