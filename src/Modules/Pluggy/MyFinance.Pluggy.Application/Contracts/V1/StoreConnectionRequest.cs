namespace MyFinance.Pluggy.Application.Contracts.V1;

public sealed record StoreConnectionRequest
{
    public string ItemId { get; init; } = null!;
    public Guid UserId { get; init; }
}
