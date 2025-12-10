using Microsoft.AspNetCore.Routing;

namespace MyFinance.Common.Application;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder builder);
}
