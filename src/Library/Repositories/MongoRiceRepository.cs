using CSharpFunctionalExtensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRice.Attributes;
using MongoRice.Configurations;
using MongoRice.Documents;
using MongoRice.Entities;
using System.Linq.Expressions;

namespace MongoRice.Repositories
{
    public class MongoRiceRepository<TDocument> : IMongoRiceRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public MongoRiceRepository(IMongoConfiguration settings)
        {
            IMongoDatabase database = new MongoClient(settings.ConnectionString).GetDatabase(settings.Database);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        public virtual IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public async virtual Task<IEnumerable<TDocument>> Find(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async virtual Task<Maybe<TDocument>> FindOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
        {
            return Maybe.From(await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken));
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

        public virtual async Task<ICollection<TDocument>> InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken = default)
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

        public async Task DeleteMany(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteManyAsync(filter, cancellationToken);
        }

        public async Task DeleteOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
        {
            await _collection.FindOneAndDeleteAsync(filter, null, cancellationToken);
        }

        public async Task<PaginatedResult<TDocument>> Find(FilterDefinition<TDocument> filter,
                                                                                       SortDefinition<TDocument> sort,
                                                                                       int page,
                                                                                       int pageSize,
                                                                                       CancellationToken cancellationToken = default)
        {
            AggregateFacet<TDocument, AggregateCountResult> countFacet =
                AggregateFacet.Create("count",
                                      PipelineDefinition<TDocument, AggregateCountResult>.Create(new[] { PipelineStageDefinitionBuilder.Count<TDocument>() }));

            AggregateFacet<TDocument, TDocument> dataFacet =
                AggregateFacet.Create("data",
                                      PipelineDefinition<TDocument, TDocument>.Create(new[]
                                      {
                                          PipelineStageDefinitionBuilder.Sort(sort),
                                          PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                                          PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize)
                                      }));


            List<AggregateFacetResults> aggregation = await _collection.Aggregate().Match(filter)
                                                                                   .Facet(countFacet, dataFacet)
                                                                                   .ToListAsync(cancellationToken);

            long? count = aggregation.First()
                                     .Facets
                                     .First(x => x.Name == "count")
                                     .Output<AggregateCountResult>()?
                                     .AsQueryable()
                                     .FirstOrDefault()?
                                     .Count;

            int totalPages = (int)Math.Ceiling((double)count / pageSize);

            IReadOnlyList<TDocument> data = aggregation.First()
                                                       .Facets
                                                       .First(x => x.Name == "data")
                                                       .Output<TDocument>();

            return new PaginatedResult<TDocument>() { PageIndex = page, Result = data, TotalPages = totalPages };
        }
        private static string GetCollectionName(Type documentType)
        {
            return ((CollectionAttribute)documentType.GetCustomAttributes(typeof(CollectionAttribute), true)
                                                     .FirstOrDefault())?.CollectionName;
        }


    }
}