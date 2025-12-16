using Bogus;
using FluentAssertions;
using MyFinance.Ledger.Domain.Transactions;

namespace MyFinance.Ledger.UnitTests.Transactions;

public class TransactionTests
{
    private readonly Faker _faker;

    public TransactionTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldCreateTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        var date = _faker.Date.PastOffset();
        var amount = _faker.Finance.Amount(1, 10000);
        var type = _faker.PickRandom<TransactionType>();
        var categoryId = Guid.NewGuid();
        var description = _faker.Lorem.Sentence();

        // Act
        var transaction = new Transaction(
            id,
            date.DateTime,
            amount,
            type,
            categoryId,
            description);

        // Assert
        transaction.Id.Should().Be(id);
        transaction.Date.Should().Be(date.DateTime);
        transaction.Amount.Should().Be(amount);
        transaction.Type.Should().Be(type);
        transaction.CategoryId.Should().Be(categoryId);
        transaction.Description.Should().Be(description);
        transaction.IsRecurring.Should().BeFalse();
        transaction.RecurrenceRule.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithRecurringTransaction_ShouldSetRecurrenceProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var date = _faker.Date.PastOffset();
        var amount = _faker.Finance.Amount(1, 10000);
        var type = _faker.PickRandom<TransactionType>();
        var categoryId = Guid.NewGuid();
        var recurrenceRule = "FREQ=MONTHLY;INTERVAL=1";

        // Act
        var transaction = new Transaction(
            id,
            date.DateTime,
            amount,
            type,
            categoryId,
            isRecurring: true,
            recurrenceRule: recurrenceRule);

        // Assert
        transaction.IsRecurring.Should().BeTrue();
        transaction.RecurrenceRule.Should().Be(recurrenceRule);
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldCreateTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        var date = _faker.Date.PastOffset();
        var amount = _faker.Finance.Amount(1, 10000);
        var type = _faker.PickRandom<TransactionType>();
        var categoryId = Guid.NewGuid();

        // Act
        var transaction = new Transaction(
            id,
            date.DateTime,
            amount,
            type,
            categoryId);

        // Assert
        transaction.Description.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var date = _faker.Date.PastOffset();
        var negativeAmount = -_faker.Finance.Amount(1, 10000);
        var type = _faker.PickRandom<TransactionType>();
        var categoryId = Guid.NewGuid();

        // Act
        var act = () => new Transaction(
            id,
            date.DateTime,
            negativeAmount,
            type,
            categoryId);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("amount");
    }

    [Fact]
    public void Constructor_WithZeroAmount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var date = _faker.Date.PastOffset();
        var zeroAmount = 0m;
        var type = _faker.PickRandom<TransactionType>();
        var categoryId = Guid.NewGuid();

        // Act
        var act = () => new Transaction(
            id,
            date.DateTime,
            zeroAmount,
            type,
            categoryId);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("amount");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateTransaction()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        var newDate = _faker.Date.FutureOffset();
        var newAmount = _faker.Finance.Amount(1, 10000);
        var newCategoryId = Guid.NewGuid();
        var newDescription = _faker.Lorem.Sentence();

        // Act
        transaction.Update(
            newDate.DateTime,
            newAmount,
            newCategoryId,
            newDescription);

        // Assert
        transaction.Date.Should().Be(newDate.DateTime);
        transaction.Amount.Should().Be(newAmount);
        transaction.CategoryId.Should().Be(newCategoryId);
        transaction.Description.Should().Be(newDescription);
    }

    [Fact]
    public void Update_WithNullDescription_ShouldUpdateTransaction()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        var newDate = _faker.Date.FutureOffset();
        var newAmount = _faker.Finance.Amount(1, 10000);
        var newCategoryId = Guid.NewGuid();

        // Act
        transaction.Update(
            newDate.DateTime,
            newAmount,
            newCategoryId,
            null);

        // Assert
        transaction.Description.Should().BeNull();
    }

    [Fact]
    public void Update_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        var newDate = _faker.Date.FutureOffset();
        var negativeAmount = -_faker.Finance.Amount(1, 10000);
        var newCategoryId = Guid.NewGuid();

        // Act
        var act = () => transaction.Update(
            newDate.DateTime,
            negativeAmount,
            newCategoryId,
            null);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("amount");
    }

    [Fact]
    public void Update_WithZeroAmount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        var newDate = _faker.Date.FutureOffset();
        var zeroAmount = 0m;
        var newCategoryId = Guid.NewGuid();

        // Act
        var act = () => transaction.Update(
            newDate.DateTime,
            zeroAmount,
            newCategoryId,
            null);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("amount");
    }

    [Fact]
    public void Update_ShouldNotChangeTransactionType()
    {
        // Arrange
        var transaction = CreateValidTransaction();
        var originalType = transaction.Type;
        var newDate = _faker.Date.FutureOffset();
        var newAmount = _faker.Finance.Amount(1, 10000);
        var newCategoryId = Guid.NewGuid();

        // Act
        transaction.Update(
            newDate.DateTime,
            newAmount,
            newCategoryId,
            null);

        // Assert
        transaction.Type.Should().Be(originalType);
    }

    [Fact]
    public void Update_ShouldNotChangeRecurrenceProperties()
    {
        // Arrange
        var recurrenceRule = "FREQ=MONTHLY;INTERVAL=1";
        var transaction = new Transaction(
            Guid.NewGuid(),
            _faker.Date.PastOffset().DateTime,
            _faker.Finance.Amount(1, 10000),
            _faker.PickRandom<TransactionType>(),
            Guid.NewGuid(),
            isRecurring: true,
            recurrenceRule: recurrenceRule);

        var newDate = _faker.Date.FutureOffset();
        var newAmount = _faker.Finance.Amount(1, 10000);
        var newCategoryId = Guid.NewGuid();

        // Act
        transaction.Update(
            newDate.DateTime,
            newAmount,
            newCategoryId,
            null);

        // Assert
        transaction.IsRecurring.Should().BeTrue();
        transaction.RecurrenceRule.Should().Be(recurrenceRule);
    }

    private Transaction CreateValidTransaction()
    {
        return new Transaction(
            Guid.NewGuid(),
            _faker.Date.PastOffset().DateTime,
            _faker.Finance.Amount(1, 10000),
            _faker.PickRandom<TransactionType>(),
            Guid.NewGuid(),
            _faker.Lorem.Sentence());
    }
}
