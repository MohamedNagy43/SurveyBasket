namespace SurveyBasket.Api.Abstractions;

public class PaginatedList<T>(List<T> items, int pageNumber, int count, int pageSize)
{
    public List<T> Items { get; private set; } = items;
    public int PageNumber { get; private set; } = pageNumber;
    public int TotalPages { get; private set; } = (int)Math.Ceiling((double)count / pageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        int count = await query.CountAsync(cancellationToken);

        List<T> items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, pageNumber, count, pageSize);
    }
}
