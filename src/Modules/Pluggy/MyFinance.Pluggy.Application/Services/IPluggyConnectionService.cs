namespace MyFinance.Pluggy.Application.Services;

public interface IPluggyConnectionService
{
    Task<string> CreateConnectTokenAsync(CancellationToken cancellationToken = default);
    Task<(Guid ConnectionId, string ConnectorName)> StoreConnectionAsync(string itemId, Guid userId, CancellationToken cancellationToken = default);
}
