namespace MongoRice.Entities
{
    public class PaginatedResult<TDocument>
    {
        public bool HasNextPage => PageNumber < TotalPages;

        public bool HasPreviousPage => PageNumber > 1;

        public IEnumerable<TDocument> Items { get; }

        public int PageNumber { get; }

        public long TotalCount { get; }

        public int TotalPages { get; }

        public PaginatedResult(IEnumerable<TDocument> items, long count, int pageNumber, int pageSize)
        {
            Items = items;
            PageNumber = pageNumber;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}