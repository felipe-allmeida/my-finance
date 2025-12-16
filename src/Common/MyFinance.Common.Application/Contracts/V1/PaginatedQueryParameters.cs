using Microsoft.AspNetCore.Mvc;

namespace MyFinance.Common.Application.Contracts.V1;

public record PaginatedQueryParameters
{
    [FromQuery]
    public int? Skip { get; set; }
    [FromQuery]
    public int? Take { get; set; }

    public PagingParameters Pagination => new PagingParameters()
    {
        Skip = this.Skip ?? 0,
        Take = this.Take ?? 50
    };
}
