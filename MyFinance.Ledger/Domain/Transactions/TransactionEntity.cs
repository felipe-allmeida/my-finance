namespace MyFinance.Ledger.Domain.Transactions;

public sealed class Transaction
{
    public Guid Id { get; private set; }
    public DateTime Date { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }

    public Guid CategoryId { get; private set; }
    public string? Description { get; private set; }

    public bool IsRecurring { get; private set; }
    public string? RecurrenceRule { get; private set; }

    private Transaction() { }

    public Transaction(
        Guid id,
        DateTime date,
        decimal amount,
        TransactionType type,
        Guid categoryId,
        string? description = null,
        bool isRecurring = false,
        string? recurrenceRule = null)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

        Id = id;
        Date = date;
        Amount = amount;
        Type = type;
        CategoryId = categoryId;
        Description = description;
        IsRecurring = isRecurring;
        RecurrenceRule = recurrenceRule;
    }

    public void Update(
        DateTime date,
        decimal amount,
        Guid categoryId,
        string? description)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

        Date = date;
        Amount = amount;
        CategoryId = categoryId;
        Description = description;
    }
}