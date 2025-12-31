using MyFinance.Common.Domain;

namespace MyFinance.Pluggy.Domain.SyncHistory;

public sealed class SyncHistory : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string ItemId { get; private set; } = null!;
    public SyncTrigger Trigger { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public int TransactionsFetched { get; private set; }
    public int TransactionsCreated { get; private set; }
    public int TransactionsFailed { get; private set; }
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }

    private SyncHistory() { }

    public SyncHistory(Guid id, Guid userId, string itemId, SyncTrigger trigger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);

        Id = id;
        UserId = userId;
        ItemId = itemId;
        Trigger = trigger;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public void Complete(int fetched, int created, int failed, string? error = null)
    {
        CompletedAt = DateTimeOffset.UtcNow;
        TransactionsFetched = fetched;
        TransactionsCreated = created;
        TransactionsFailed = failed;
        Success = error == null;
        ErrorMessage = error;
    }
}
