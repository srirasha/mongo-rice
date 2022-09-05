using MongoDB.Bson;
using System;

namespace MongoRice.Documents
{
    public class Document : IDocument
    {
        public DateTime CreatedAt => Id.CreationTime;

        public ObjectId Id { get; set; }

    }
}