using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MyFinance.Common.Application;
using MyFinance.Common.Domain;
using MyFinance.Ledger.Application.Contracts;
using MyFinance.Ledger.Domain.Transactions;

namespace MyFinance.Ledger.Application.Transactions;

internal sealed class CreateTransaction : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder) =>
        builder.MapPost("", Handle)
            .WithName("CreateTransaction")
            .WithSummary("Create an transaction")
            .WithDescription("Create an `transaction`")
            .ProducesValidationProblem()
            .Produces<Guid>();

    private static async Task<IResult> Handle(
        [FromBody] CreateTransactionRequest request,
        [FromServices] IUnitOfWork uow,
        [FromServices] ILogger<CreateTransaction> logger,
        CancellationToken ct)
    {
        var transaction = new Transaction(
            id: TransactionId.New(),
            date: request.Date,
            amount: request.Amount,
            description: request.Description,
            type: request.TransactionType,
            categoryId: request.CategoryId,
            isRecurring: request.IsRecurring,
            recurrenceRule: request.RecurrenceRule
            );

        await uow.AddAsync(transaction, ct);

        await uow.SaveChangesAsync(ct);

        return TypedResults.Ok(transaction.Id);
    }

    public record CreateTransactionRequest
    {
        public DateTime Date { get; init; }
        public decimal Amount { get; init; }
        public string? Description { get; init; }
        public TransactionType TransactionType { get; init; }
        public CategoryId CategoryId { get; init; }

        public bool IsRecurring { get; init; }
        public string? RecurrenceRule { get; init; }
    }
}
