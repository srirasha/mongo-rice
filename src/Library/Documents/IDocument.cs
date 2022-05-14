using MongoDB.Bson;

namespace Library.Documents
{
    public interface IDocument
    {
        ObjectId Id { get; set; }
    }
}