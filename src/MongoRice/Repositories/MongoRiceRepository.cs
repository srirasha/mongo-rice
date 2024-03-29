﻿using CSharpFunctionalExtensions;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRice.Attributes;
using MongoRice.Configurations;
using MongoRice.Documents;
using MongoRice.Entities;
using MongoRice.Validations.Attributes;
using MongoRice.Validations.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MongoRice.Repositories
{
    public class MongoRiceRepository<TDocument> : IMongoRiceRepository<TDocument> where TDocument : IDocument
    {
        private readonly IValidator<IMongoConfiguration> _mongoConfigurationValidator;
        private readonly IValidator<CollectionAttribute> _collectionAttributeValidator;

        private readonly FilterDefinition<TDocument> _emptyFilterDefinition = Builders<TDocument>.Filter.Empty;
        private readonly SortDefinition<TDocument> _defaultSortDefinition = Builders<TDocument>.Sort.Descending(f => f.Id);

        public IMongoCollection<TDocument> Collection { get; }

        public MongoRiceRepository(IMongoConfiguration configuration)
        {
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

        public virtual async Task DeleteMany(Expression<Func<TDocument, bool>> filter = null,
                                             DeleteOptions options = null,
                                             CancellationToken cancellationToken = default)
        {
            await Collection.DeleteManyAsync(filter ?? _emptyFilterDefinition, options, cancellationToken);
        }

        public virtual async Task<TDocument> DeleteOne(FilterDefinition<TDocument> filter,
                                                       FindOneAndDeleteOptions<TDocument> options = null,
                                                       CancellationToken cancellationToken = default)
        {
            return await Collection.FindOneAndDeleteAsync(filter, options, cancellationToken);
        }

        public virtual async Task<TDocument> DeleteOne(Expression<Func<TDocument, bool>> filter = null,
                                                       FindOneAndDeleteOptions<TDocument> options = null,
                                                       CancellationToken cancellationToken = default)
        {
            return await Collection.FindOneAndDeleteAsync(filter, options, cancellationToken);
        }

        public virtual async Task<IEnumerable<TDocument>> Find(FilterDefinition<TDocument> filter = null,
                                                               FindOptions options = null,
                                                               SortDefinition<TDocument> sort = null,
                                                               CancellationToken cancellationToken = default)
        {
            return await Collection.Find(filter ?? _emptyFilterDefinition, options)
                         .Sort(sort ?? _defaultSortDefinition)
                         .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TDocument>> Find(Expression<Func<TDocument, bool>> filter = null,
                                                               FindOptions options = null,
                                                               SortOptions<TDocument> sort = null,
                                                               CancellationToken cancellationToken = default)
        {
            return await Collection.Find(filter ?? _emptyFilterDefinition, options)
                                   .Sort(sort == null ? _defaultSortDefinition : SortOptions<TDocument>.BuildSortFilter(sort))
                                   .ToListAsync(cancellationToken);
        }

        public virtual async Task<PaginatedResult<TDocument>> Find(int page,
                                                                   int pageSize,
                                                                   FilterDefinition<TDocument> filter = null,
                                                                   FindOptions options = null,
                                                                   SortDefinition<TDocument> sort = null,
                                                                   CancellationToken cancellationToken = default)
        {
            string countAggregateName = "count";
            string dataAggregateName = "data";

            AggregateFacet<TDocument, AggregateCountResult> countFacet =
                AggregateFacet.Create(countAggregateName,
                                      PipelineDefinition<TDocument, AggregateCountResult>.Create(new[] { PipelineStageDefinitionBuilder.Count<TDocument>() }));

            AggregateFacet<TDocument, TDocument> dataFacet =
                AggregateFacet.Create(dataAggregateName,
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
                                     .First(x => x.Name == countAggregateName)
                                     .Output<AggregateCountResult>()?
                                     .AsQueryable()
                                     .FirstOrDefault()?
                                     .Count;

            int totalPages = (int)Math.Ceiling((double)count / pageSize);

            IEnumerable<TDocument> data = aggregation.First()
                                                       .Facets
                                                       .First(x => x.Name == dataAggregateName)
                                                       .Output<TDocument>();

            return new PaginatedResult<TDocument>(data, count.Value, page, pageSize);
        }

        public virtual async Task<PaginatedResult<TDocument>> Find(int page,
                                                                   int pageSize,
                                                                   Expression<Func<TDocument, bool>> filter = null,
                                                                   FindOptions options = null,
                                                                   SortOptions<TDocument> sort = null,
                                                                   CancellationToken cancellationToken = default)
        {
            string countAggregateName = "count";
            string dataAggregateName = "data";

            AggregateFacet<TDocument, AggregateCountResult> countFacet =
                AggregateFacet.Create(countAggregateName,
                                      PipelineDefinition<TDocument, AggregateCountResult>.Create(new[] { PipelineStageDefinitionBuilder.Count<TDocument>() }));

            AggregateFacet<TDocument, TDocument> dataFacet =
                AggregateFacet.Create(dataAggregateName,
                                      PipelineDefinition<TDocument, TDocument>.Create(new[]
                                      {
                                          PipelineStageDefinitionBuilder.Sort(sort == null? _defaultSortDefinition : SortOptions<TDocument>.BuildSortFilter(sort)),
                                          PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                                          PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize)
                                      }));


            List<AggregateFacetResults> aggregation = await Collection.Aggregate().Match(filter ?? _emptyFilterDefinition)
                                                                                  .Facet(countFacet, dataFacet)
                                                                                  .ToListAsync(cancellationToken);

            long? count = aggregation.First()
                                     .Facets
                                     .First(x => x.Name == countAggregateName)
                                     .Output<AggregateCountResult>()?
                                     .AsQueryable()
                                     .FirstOrDefault()?
                                     .Count;

            int totalPages = (int)Math.Ceiling((double)count / pageSize);

            IEnumerable<TDocument> data = aggregation.First()
                                                       .Facets
                                                       .First(x => x.Name == dataAggregateName)
                                                       .Output<TDocument>();

            return new PaginatedResult<TDocument>(data, count.Value, page, pageSize);
        }
        public virtual async Task<IEnumerable<TDocument>> FindAll(SortDefinition<TDocument> sort = null,
                                                                  FindOptions options = null,
                                                                  CancellationToken cancellationToken = default)
        {
            return await Collection.Find(_emptyFilterDefinition, options)
                                   .Sort(sort ?? _defaultSortDefinition)
                                   .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TDocument>> FindAll(SortOptions<TDocument> sort = null,
                                                          FindOptions options = null,
                                                          CancellationToken cancellationToken = default)
        {
            return await Collection.Find(_emptyFilterDefinition, options)
                                   .Sort(sort == null ? _defaultSortDefinition : SortOptions<TDocument>.BuildSortFilter(sort))
                                   .ToListAsync(cancellationToken);
        }

        public virtual async Task<Maybe<TDocument>> FindOne(FilterDefinition<TDocument> filter,
                                                            FindOptions options = null,
                                                            CancellationToken cancellationToken = default)
        {
            return Maybe.From(await Collection.Find(filter, options).FirstOrDefaultAsync(cancellationToken));
        }

        public virtual async Task<Maybe<TDocument>> FindOne(Expression<Func<TDocument, bool>> filter,
                                                            FindOptions options = null,
                                                            CancellationToken cancellationToken = default)
        {
            return Maybe.From(await Collection.Find(filter, options).FirstOrDefaultAsync(cancellationToken));
        }

        public virtual async Task<Maybe<TDocument>> FindById(string id,
                                                             FindOptions options = null,
                                                             CancellationToken cancellationToken = default)
        {
            ObjectId objectId = new(id);
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            return Maybe.From(await Collection.Find(filter, options).SingleOrDefaultAsync(cancellationToken));
        }

        public virtual async Task<TDocument> InsertOne(TDocument document,
                                                       InsertOneOptions options = null,
                                                       CancellationToken cancellationToken = default)
        {
            await Collection.InsertOneAsync(document, options, cancellationToken);

            return document;
        }

        public virtual async Task<IEnumerable<TDocument>> InsertMany(ICollection<TDocument> documents,
                                                                     InsertManyOptions options = null,
                                                                     CancellationToken cancellationToken = default)
        {
            await Collection.InsertManyAsync(documents, options, cancellationToken);

            return documents;
        }

        public virtual async Task<TDocument> ReplaceOne(TDocument document,
                                                        FindOneAndReplaceOptions<TDocument> options = null,
                                                        CancellationToken cancellationToken = default)
        {
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);

            return await Collection.FindOneAndReplaceAsync(filter,
                                                           document,
                                                           options ?? new FindOneAndReplaceOptions<TDocument>() { ReturnDocument = ReturnDocument.After },
                                                           cancellationToken);
        }

        private string GetCollectionName(Type documentType)
        {
            CollectionAttribute collectionAttribute = (CollectionAttribute)documentType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault();

            _collectionAttributeValidator.ValidateAndThrow(collectionAttribute);

            return collectionAttribute.CollectionName;
        }
    }
}