using MyFinance.Common.Domain;

namespace MyFinance.Ledger.Domain.Transactions;

public class Transaction : Entity
{
    public Guid Id { get; private set; }
    public DateTimeOffset Date { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }

    public Guid CategoryId { get; private set; }
    public string? Description { get; private set; }

    public bool IsRecurring { get; private set; }
    public string? RecurrenceRule { get; private set; }

    public TransactionSource Source { get; private set; }
    public string? ExternalId { get; private set; }
    public string? ExternalSource { get; private set; }

    protected Transaction() { }

    public Transaction(
        Guid id,
        DateTime date,
        decimal amount,
        TransactionType type,
        Guid categoryId,
        string? description = null,
        bool isRecurring = false,
        string? recurrenceRule = null,
        TransactionSource source = TransactionSource.Manual,
        string? externalId = null,
        string? externalSource = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Id = id;
        Date = date;
        Amount = amount;
        Type = type;
        CategoryId = categoryId;
        Description = description;
        IsRecurring = isRecurring;
        RecurrenceRule = recurrenceRule;
        Source = source;
        ExternalId = externalId;
        ExternalSource = externalSource;
    }

    public void Update(
        DateTime date,
        decimal amount,
        Guid categoryId,
        string? description)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Date = date;
        Amount = amount;
        CategoryId = categoryId;
        Description = description;
    }
}

public enum TransactionSource
{
    Manual,
    Pluggy,
    Import
}
