using Microsoft.EntityFrameworkCore;

namespace E_Shop.Core.Abstractions;

public class PaginatedData<T> (List<T> Items , int PageNumber , int PageSize , int count)
{
    public List<T> Items { get; private set; } = Items;
    public int PageNumber { get; private set; } = PageNumber;
    public int PageSize { get; private set; } = PageSize;
    public int TotalPages { get; private set; } = (int)Math.Ceiling(count / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
    public static async Task<PaginatedData<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize , CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedData<T>(items, pageNumber, pageSize, count);
    }

}
