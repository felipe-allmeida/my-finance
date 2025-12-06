using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFinance.Common.Application;
using MyFinance.Common.Domain;
using MyFinance.Ledger.Domain.Transactions;

namespace MyFinance.Ledger.Application.Transactions;

internal sealed class CreateTransaction : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) =>
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
        var transaction = new Transaction(id: TransactionId.New());

        await uow.AddAsync(transaction, ct);

        return TypedResults.Ok(response);
    }

    public record CreateTransactionRequest
    {

    }
}
