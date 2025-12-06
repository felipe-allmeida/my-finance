using Microsoft.AspNetCore.Routing;

namespace MyFinance.Common.Application;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder builder);
}