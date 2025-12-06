using MyFinance.Common.Domain;

namespace MyFinance.Ledger.Domain.Categories;

public sealed class Category : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public FlowType FlowType { get; private set; }
    public bool IsEssential { get; private set; }

    private Category() { }

    public Category(Guid id, string name, FlowType flowType, bool isEssential)
    {
        Id = id;
        Name = name;
        FlowType = flowType;
        IsEssential = isEssential;
    }

    public void Rename(string name) => Name = name;
}
