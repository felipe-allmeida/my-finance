using Microsoft.EntityFrameworkCore;
using MyFinance.Common.Application.Contracts.V1;

namespace MyFinance.Common.Application.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedList<T>> PaginateAsync<T>(
        this IQueryable<T> queryable,
        PagingParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var records = await queryable.CountAsync(cancellationToken);

        if (records == 0)
        {
            return PagedList<T>.Empty();
        }

        if (parameters.Take < 0)
        {
            return new PagedList<T>
            {
                Items = await queryable.ToListAsync(cancellationToken),
                TotalItems = records,
                TotalPages = 1
            };
        }

        var results = await queryable
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToListAsync(cancellationToken);

        var totalPages = parameters.Take > 0
            ? (int)Math.Ceiling(records / (double)parameters.Take)
            : 1;

        return new PagedList<T>
        {
            Items = results,
            TotalItems = records,
            TotalPages = totalPages
        };
    }
}
