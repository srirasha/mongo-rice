using CSharpFunctionalExtensions;
using Library.Attributes;
using Library.Configurations;
using Library.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Library.Repositories
{
    public class MongoRiceRepository<TDocument> : IMongoRiceRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public MongoRiceRepository(MongoConfiguration settings)
        {
            IMongoDatabase database = new MongoClient(settings.ConnectionString).GetDatabase(settings.Database);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        public virtual IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public async virtual Task<IEnumerable<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(filterExpression).ToListAsync(cancellationToken);
        }

        public async virtual Task<Maybe<TDocument>> FindOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return Maybe.From(await _collection.Find(filterExpression).FirstOrDefaultAsync(cancellationToken));
        }

        public async virtual Task<Maybe<TDocument>> FindById(string id, CancellationToken cancellationToken = default)
        {
            ObjectId objectId = new(id);
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            return Maybe.From(await _collection.Find(filter).SingleOrDefaultAsync(cancellationToken));
        }

        public async virtual Task<TDocument> InsertOne(TDocument document, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(document, null, cancellationToken);
            return document;
        }

        public virtual async Task<ICollection<TDocument>> InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken)
        {
            await _collection.InsertManyAsync(documents, null, cancellationToken);
            return documents;
        }

        public virtual async Task ReplaceOne(TDocument document, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document, null, cancellationToken);
        }
        
        public async Task DeleteById(string id, CancellationToken cancellationToken = default)
        {
            ObjectId objectId = new(id);
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            await _collection.FindOneAndDeleteAsync(filter, null, cancellationToken);
        }

        public async Task DeleteMany(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteManyAsync(filterExpression, cancellationToken);
        }

        public async Task DeleteOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            await _collection.FindOneAndDeleteAsync(filterExpression, null, cancellationToken);
        }

        private static string GetCollectionName(Type documentType)
        {
            return ((CollectionAttribute)documentType.GetCustomAttributes(typeof(CollectionAttribute), true)
                                                     .FirstOrDefault())?.CollectionName;
        }
    }
}