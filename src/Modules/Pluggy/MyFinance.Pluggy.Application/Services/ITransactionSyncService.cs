namespace MyFinance.Pluggy.Application.Services;

public interface ITransactionSyncService
{
    Task SyncTransactionsAsync(string itemId, Guid userId, CancellationToken cancellationToken = default);
}
