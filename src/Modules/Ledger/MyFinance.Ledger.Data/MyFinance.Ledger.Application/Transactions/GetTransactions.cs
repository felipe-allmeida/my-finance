using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyFinance.Common.Application;
using MyFinance.Common.Application.Contracts.V1;
using MyFinance.Common.Domain;
using MyFinance.Ledger.Application.Contracts.V1;
using MyFinance.Common.Application.Extensions;
using MyFinance.Ledger.Domain.Transactions;
using MyFinance.Ledger.Application.Contracts;

namespace MyFinance.Ledger.Application.Transactions;

internal sealed class GetTransactions : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("", Handle)
            .WithName("GetTransactions")
            .WithSummary("Get all transactions")
            .WithDescription("Retrieve a list of all `transactions`")
            .ProducesValidationProblem()
            .Produces<PagedList<TransactionResponse>>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetTransactionsQueryParameters queryParameters,
        [FromServices] IUnitOfWork uow,
        CancellationToken ct)
    {
        var transactionsQuery = uow.ReadSet<Transaction>();

        if (queryParameters.CategoryId.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.CategoryId == queryParameters.CategoryId.Value.RawId);
        }

        if (queryParameters.StartDate.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.Date >= queryParameters.StartDate.Value);
        }

        if (queryParameters.EndDate.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.Date <= queryParameters.EndDate.Value);
        }

        var transactions = await transactionsQuery.PaginateAsync(queryParameters.Pagination, ct);

        return TypedResults.Ok(transactions);
    }

    public record GetTransactionsQueryParameters : PaginatedQueryParameters
    {
        [FromQuery]
        public CategoryId? CategoryId { get; init; }

        [FromQuery]
        public DateTimeOffset? StartDate { get; init; }

        [FromQuery]
        public DateTimeOffset? EndDate { get; init; }
    }

}
