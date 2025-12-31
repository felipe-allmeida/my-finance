using MyFinance.Common.Domain;

namespace MyFinance.Pluggy.Domain.Connections;

public sealed class UserConnection : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string ItemId { get; private set; } = null!;
    public string ConnectorName { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private UserConnection() { }

    public UserConnection(
        Guid id,
        Guid userId,
        string itemId,
        string connectorName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectorName);

        Id = id;
        UserId = userId;
        ItemId = itemId;
        ConnectorName = connectorName;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
