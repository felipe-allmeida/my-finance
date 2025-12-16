using MyFinance.Ledger.Domain.Transactions;

namespace MyFinance.Ledger.Application.Contracts.V1;

public record TransactionResponse
{
    public TransactionId Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; } = null!;
    public bool IsRecurring { get; set; }
    public string? RecurrenceRule { get; set; }
}
