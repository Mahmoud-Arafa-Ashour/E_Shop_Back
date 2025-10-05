namespace E_Shop.Core.Abstractions
{
    public record RequestedFilters
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
