namespace MongoRice.Entities
{
    public class PaginatedResult<TDocument>
    {
        public int PageIndex { get; set; }

        public IReadOnlyList<TDocument> Result { get; set; }

        public int TotalPages { get; set; }
    }
}