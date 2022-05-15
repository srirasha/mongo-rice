namespace MongoRice.Entities
{
    public class PaginatedResult<TDocument>
    {
        public long Count { get; set; }

        public int PageIndex { get; set; }

        public IReadOnlyList<TDocument> Result { get; set; }

        public int TotalPages { get; set; }
    }
}