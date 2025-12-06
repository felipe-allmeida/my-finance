using Microsoft.AspNetCore.Routing;

namespace MyFinance.Common.Api;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder builder);
}