using MongoDB.Bson;
using MongoRice.Documents;

namespace MongoRice.Tests.Mocks
{
    public class MockEntityDocument : IDocument
    {
        public ObjectId Id { get; set; }
    }
}