namespace MyFinance.Pluggy.Application.Contracts.Pluggy;

public sealed record PluggyTransactionResponse
{
    public string Id { get; init; } = null!;
    public string AccountId { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string Type { get; init; } = null!;
    public string? Category { get; init; }
    public string? CurrencyCode { get; init; }
    public decimal? Balance { get; init; }
    public string? ProviderCode { get; init; }
}

public sealed record PluggyTransactionsPageResponse
{
    public List<PluggyTransactionResponse> Results { get; init; } = [];
    public int Total { get; init; }
    public int Page { get; init; }
    public int TotalPages { get; init; }
}
