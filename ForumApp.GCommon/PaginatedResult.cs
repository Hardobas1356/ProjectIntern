using Microsoft.EntityFrameworkCore;

namespace ForumApp.GCommon;

public class PaginatedResult<T>
{
    public ICollection<T> Items { get; private set; } = new HashSet<T>();
    public int TotalItems { get; private set; }
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }

    public bool HasPreviousPage => (PageIndex > 1);
    public bool HasNextPage => (PageIndex < TotalPages);
    public int FirstItemIndex => (PageIndex - 1) * PageSize + 1;
    public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalItems);

    public PaginatedResult(ICollection<T> items, int totalItems,
        int pageIndex, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);
    }

    public static async Task<PaginatedResult<T>> CreateAsync
        (IQueryable<T> source, int pageIndex, int pageSize)
    {
        int count = await source.CountAsync();
        ICollection<T> items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, count, pageIndex, pageSize);
    }
}