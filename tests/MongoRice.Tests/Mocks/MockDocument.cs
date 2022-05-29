using MongoDB.Bson;
using MongoRice.Documents;

namespace MongoRice.Tests.Mocks
{
    public class MockDocument : IDocument
    {
        public ObjectId Id { get; set; }
    }
}