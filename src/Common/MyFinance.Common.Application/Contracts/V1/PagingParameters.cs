namespace MyFinance.Common.Application.Contracts.V1;

public record PagingParameters
{
    public int Skip { get; init; } = 0;

    public int Take { get; init; } = 50;
}
