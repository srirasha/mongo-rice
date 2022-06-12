using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRice.Attributes;
using MongoRice.Configurations;
using MongoRice.Documents;
using MongoRice.Entities;
using MongoRice.Validations.Attributes;
using MongoRice.Validations.Configurations;
using System.Linq.Expressions;

namespace MongoRice.Repositories
{
    public class MongoRiceRepository<TEntity, TDocument> : IMongoRiceRepository<TEntity, TDocument>
                                                           where TEntity : class, new()
                                                           where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;
        private readonly IMapper _mapper;
        private readonly IValidator<IMongoConfiguration> _mongoConfigurationValidator;
        private readonly IValidator<CollectionAttribute> _collectionAttributeValidator;

        public MongoRiceRepository(IMongoConfiguration configuration, IMapper mapper)
        {
            _mapper = mapper;
            _mongoConfigurationValidator = new MongoConfigurationValidator();
            _collectionAttributeValidator = new CollectionAttributeValidator();

            _mongoConfigurationValidator.ValidateAndThrow(configuration);

            IMongoDatabase database = new MongoClient(configuration.ConnectionString).GetDatabase(configuration.Database);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        public virtual IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public async virtual Task<IEnumerable<TEntity>> Find(FilterDefinition<TDocument> filter, SortDefinition<TDocument> sort = null, CancellationToken cancellationToken = default)
        {
            return _mapper.Map<IEnumerable<TEntity>>(await _collection.Find(filter)
                          .Sort(sort)
                          .ToListAsync(cancellationToken));
        }

        public async virtual Task<IEnumerable<TEntity>> Find(Expression<Func<TDocument, bool>> filter, SortDefinition<TDocument> sort = null, CancellationToken cancellationToken = default)
        {
            return _mapper.Map<IEnumerable<TEntity>>(await _collection.Find(filter)
                          .Sort(sort)
                          .ToListAsync(cancellationToken));
        }

        public async virtual Task<Maybe<TEntity>> FindOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
        {
            return Maybe.From(_mapper.Map<TEntity>(await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken)));
        }

        public async virtual Task<Maybe<TEntity>> FindById(string id, CancellationToken cancellationToken = default)
        {
            ObjectId objectId = new(id);
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            return Maybe.From(_mapper.Map<TEntity>(await _collection.Find(filter).SingleOrDefaultAsync(cancellationToken)));
        }

        public async virtual Task<TEntity> InsertOne(TDocument document, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(document, null, cancellationToken);
            return _mapper.Map<TEntity>(document);
        }

        public virtual async Task<IEnumerable<TEntity>> InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken = default)
        {
            await _collection.InsertManyAsync(documents, null, cancellationToken);
            return _mapper.Map<IEnumerable<TEntity>>(documents);
        }

        public virtual async Task<TEntity> ReplaceOne(TDocument document, CancellationToken cancellationToken = default)
        {
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            TDocument replacedDocument = await _collection.FindOneAndReplaceAsync(filter, document, new FindOneAndReplaceOptions<TDocument>() { ReturnDocument = ReturnDocument.After }, cancellationToken);

            return _mapper.Map<TEntity>(replacedDocument);
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

        public async Task<PaginatedResult<TEntity>> Find(FilterDefinition<TDocument> filter,
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

            return new PaginatedResult<TEntity>()
            {
                Count = count.Value,
                PageIndex = page,
                Result = _mapper.Map<IReadOnlyList<TEntity>>(data),
                TotalPages = totalPages
            };
        }
        private string GetCollectionName(Type documentType)
        {
            CollectionAttribute collectionAttribute = (CollectionAttribute)documentType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault();

            _collectionAttributeValidator.ValidateAndThrow(collectionAttribute);

            return collectionAttribute.CollectionName;
        }
    }
}