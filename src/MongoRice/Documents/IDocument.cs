using MongoDB.Bson;

namespace MongoRice.Documents
{
    public interface IDocument
    {
        ObjectId Id { get; set; }
    }
}