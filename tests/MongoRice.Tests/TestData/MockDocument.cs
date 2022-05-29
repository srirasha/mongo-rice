using MongoDB.Bson;
using MongoRice.Documents;

namespace MongoRice.Tests.TestData
{
    public class MockDocument : IDocument
    {
        public ObjectId Id { get; set; }
    }
}