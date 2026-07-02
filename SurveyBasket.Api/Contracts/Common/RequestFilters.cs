namespace SurveyBasket.Api.Contracts.Common;

public record RequestFilters
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = Pagination.DefaultPageSize;
    public string? SearchValue { get; init; }
    public string? SortColumn { get; init; }
    public string? SortDirction { get; init; } = "ASC";
}
