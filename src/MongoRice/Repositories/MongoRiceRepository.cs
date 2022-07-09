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
        private readonly IValidator<IMongoConfiguration> _mongoConfigurationValidator;
        private readonly IValidator<CollectionAttribute> _collectionAttributeValidator;

        private readonly FilterDefinition<TDocument> _emptyFilterDefinition = Builders<TDocument>.Filter.Empty;
        private readonly SortDefinition<TDocument> _defaultSortDefinition = Builders<TDocument>.Sort.Descending(f => f.Id);

        public IMongoCollection<TDocument> Collection { get; }
        public IMapper Mapper { get; set; }

        public MongoRiceRepository(IMongoConfiguration configuration, IMapper mapper)
        {
            Mapper = mapper;
            _mongoConfigurationValidator = new MongoConfigurationValidator();
            _collectionAttributeValidator = new CollectionAttributeValidator();

            _mongoConfigurationValidator.ValidateAndThrow(configuration);

            IMongoDatabase database = new MongoClient(configuration.ConnectionString).GetDatabase(configuration.Database);
            Collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        public IQueryable<TDocument> AsQueryable()
        {
            return Collection.AsQueryable();
        }

        public virtual async Task DeleteById(string id,
                                             FindOneAndDeleteOptions<TDocument> options = null,
                                             CancellationToken cancellationToken = default)
        {
            ObjectId objectId = new(id);
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            await Collection.FindOneAndDeleteAsync(filter, options, cancellationToken);
        }

        public virtual async Task DeleteMany(FilterDefinition<TDocument> filter = null,
                                             DeleteOptions options = null,
                                             CancellationToken cancellationToken = default)
        {
            await Collection.DeleteManyAsync(filter ?? _emptyFilterDefinition, options, cancellationToken);
        }

        public virtual async Task<TEntity> DeleteOne(FilterDefinition<TDocument> filter,
                                                       FindOneAndDeleteOptions<TDocument> options = null,
                                                       CancellationToken cancellationToken = default)
        {
            return Mapper.Map<TEntity>(await Collection.FindOneAndDeleteAsync(filter, options, cancellationToken));
        }

        public virtual async Task<IEnumerable<TEntity>> Find(FilterDefinition<TDocument> filter = null,
                                                             FindOptions options = null,
                                                             SortDefinition<TDocument> sort = null,
                                                             CancellationToken cancellationToken = default)
        {
            return Mapper.Map<IEnumerable<TEntity>>(await Collection.Find(filter ?? _emptyFilterDefinition, options)
                         .Sort(sort ?? _defaultSortDefinition)
                         .ToListAsync(cancellationToken));
        }

        public virtual async Task<PaginatedResult<TEntity>> Find(int page,
                                                         int pageSize,
                                                         FilterDefinition<TDocument> filter = null,
                                                         FindOptions options = null,
                                                         SortDefinition<TDocument> sort = null,
                                                         CancellationToken cancellationToken = default)
        {
            AggregateFacet<TDocument, AggregateCountResult> countFacet =
                AggregateFacet.Create("count",
                                      PipelineDefinition<TDocument, AggregateCountResult>.Create(new[] { PipelineStageDefinitionBuilder.Count<TDocument>() }));

            AggregateFacet<TDocument, TDocument> dataFacet =
                AggregateFacet.Create("data",
                                      PipelineDefinition<TDocument, TDocument>.Create(new[]
                                      {
                                          PipelineStageDefinitionBuilder.Sort(sort ?? _defaultSortDefinition),
                                          PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                                          PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize)
                                      }));


            List<AggregateFacetResults> aggregation = await Collection.Aggregate().Match(filter ?? _emptyFilterDefinition)
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

            return new PaginatedResult<TEntity>(Mapper.Map<IReadOnlyList<TEntity>>(data), count.Value, page, pageSize);
        }

        public virtual async Task<IEnumerable<TEntity>> Find(Expression<Func<TDocument, bool>> filter = null,
                                                             FindOptions options = null,
                                                             SortDefinition<TDocument> sort = null,
                                                             CancellationToken cancellationToken = default)
        {
            return Mapper.Map<IEnumerable<TEntity>>(await Collection.Find(filter ?? _emptyFilterDefinition, options)
                         .Sort(sort ?? _defaultSortDefinition)
                         .ToListAsync(cancellationToken));
        }

        public virtual async Task<IEnumerable<TEntity>> FindAll(SortDefinition<TDocument> sort = null,
                                                                FindOptions options = null,
                                                                CancellationToken cancellationToken = default)
        {
            return Mapper.Map<IEnumerable<TEntity>>(await Collection.Find(_emptyFilterDefinition, options)
                         .Sort(sort ?? _defaultSortDefinition)
                         .ToListAsync(cancellationToken));
        }

        public virtual async Task<Maybe<TEntity>> FindOne(FilterDefinition<TDocument> filter,
                                                          FindOptions options = null,
                                                          CancellationToken cancellationToken = default)
        {
            return Maybe.From(Mapper.Map<TEntity>(await Collection.Find(filter, options).FirstOrDefaultAsync(cancellationToken)));
        }

        public virtual async Task<Maybe<TEntity>> FindById(string id,
                                                           FindOptions options = null,
                                                           CancellationToken cancellationToken = default)
        {
            ObjectId objectId = new(id);
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            return Maybe.From(Mapper.Map<TEntity>(await Collection.Find(filter, options).SingleOrDefaultAsync(cancellationToken)));
        }

        public virtual async Task<TEntity> InsertOne(TDocument document,
                                                     InsertOneOptions options = null,
                                                     CancellationToken cancellationToken = default)
        {
            await Collection.InsertOneAsync(document, options, cancellationToken);
            return Mapper.Map<TEntity>(document);
        }

        public virtual async Task<IEnumerable<TEntity>> InsertMany(ICollection<TDocument> documents,
                                                                   InsertManyOptions options = null,
                                                                   CancellationToken cancellationToken = default)
        {
            await Collection.InsertManyAsync(documents, options, cancellationToken);
            return Mapper.Map<IEnumerable<TEntity>>(documents);
        }

        public virtual async Task<TEntity> ReplaceOne(TDocument document,
                                                      FindOneAndReplaceOptions<TDocument> options = null,
                                                      CancellationToken cancellationToken = default)
        {
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            TDocument replacedDocument = await Collection.FindOneAndReplaceAsync(filter,
                                                                                 document,
                                                                                 options ?? new FindOneAndReplaceOptions<TDocument>() { ReturnDocument = ReturnDocument.After },
                                                                                 cancellationToken);

            return Mapper.Map<TEntity>(replacedDocument);
        }

        private string GetCollectionName(Type documentType)
        {
            CollectionAttribute collectionAttribute = (CollectionAttribute)documentType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault();

            _collectionAttributeValidator.ValidateAndThrow(collectionAttribute);

            return collectionAttribute.CollectionName;
        }
    }
}