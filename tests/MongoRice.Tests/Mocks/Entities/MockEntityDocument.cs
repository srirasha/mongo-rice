using MongoDB.Bson;
using MongoRice.Attributes;
using MongoRice.Documents;

namespace MongoRice.Tests.Mocks.Entities
{
    [Collection("entities")]
    public class MockEntityDocument : IDocument
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}