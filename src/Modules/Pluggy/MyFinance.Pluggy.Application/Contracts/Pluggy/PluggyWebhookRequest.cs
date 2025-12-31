namespace MyFinance.Pluggy.Application.Contracts.Pluggy;

public sealed record PluggyWebhookRequest
{
    public string Event { get; init; } = null!;
    public PluggyWebhookData Data { get; init; } = null!;
}

public sealed record PluggyWebhookData
{
    public string ItemId { get; init; } = null!;
    public string? Error { get; init; }
}
