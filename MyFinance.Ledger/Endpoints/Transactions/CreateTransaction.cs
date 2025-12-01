using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyFinance.Common.Api;

namespace MyFinance.Ledger.Endpoints.Transactions;

public class CreateTransaction : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) =>
        builder.MapPost("", Handle)
            .WithRequestNormalizer<CreateEmployerRequest>()
            .WithRequestValidation<CreateEmployerRequest>()
            .WithName("CreateEmployer")
            .WithSummary("Create an employer")
            .WithDescription("Creates an `employer`.")
            .ProducesValidationProblem()
            .Produces<EmployerResponse>();
    
    private static async Task<IResult> Handle([FromBody] CreateTransactionRequest request,

    public class CreateTransactionRequest
    {
        
    }
}