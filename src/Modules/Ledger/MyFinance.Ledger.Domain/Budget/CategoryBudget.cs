using MyFinance.Common.Domain;

namespace MyFinance.Ledger.Domain.Budget;

public sealed class CategoryBudget : Entity
{
    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public int Year { get; private set; }
    public int Month { get; private set; }
    public decimal LimitAmount { get; private set; }

    private CategoryBudget() { }

    public CategoryBudget(Guid id, Guid categoryId, int year, int month, decimal limitAmount)
    {
        Id = id;
        CategoryId = categoryId;
        Year = year;
        Month = month;
        LimitAmount = limitAmount;
    }

    public void UpdateLimit(decimal newLimit) => LimitAmount = newLimit;
}