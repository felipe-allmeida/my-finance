using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MyFinance.Common.Api;

namespace MyFinance.Ledger.Presentation.Transactions;

internal sealed class CreateTransaction : IEndpoint
{
    public void Map(IEndpointRouteBuilder app) =>
        app.MapPost("", )
}