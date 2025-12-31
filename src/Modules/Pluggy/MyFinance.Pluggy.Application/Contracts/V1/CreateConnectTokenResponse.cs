namespace MyFinance.Pluggy.Application.Contracts.V1;

public sealed record CreateConnectTokenResponse
{
    public string AccessToken { get; init; } = null!;
}
