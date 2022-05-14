using MongoDB.Bson;

namespace Library.Documents
{
    public class Document : IDocument
    {
        public DateTime CreatedAt => Id.CreationTime;

        public ObjectId Id { get; set; }

    }
}