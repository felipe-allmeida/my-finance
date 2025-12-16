namespace MyFinance.Common.Application.Contracts.V1;

public record class PagedList<T>
{
    public required List<T> Items { get; init; }
    public required int TotalItems { get; init; }
    public required int TotalPages { get; init; }

    public static PagedList<T> Empty() =>
        new()
        {
            Items = [],
            TotalItems = 0,
            TotalPages = 1
        };

}
