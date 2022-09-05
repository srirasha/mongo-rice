using CSharpFunctionalExtensions;
using MongoDB.Driver;
using MongoRice.Documents;
using MongoRice.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MongoRice.Repositories
{
    public interface IMongoRiceRepository<TDocument> where TDocument : IDocument
    {
        IMongoCollection<TDocument> Collection { get; }

        Task DeleteById(string id,
                        FindOneAndDeleteOptions<TDocument> options = null,
                        CancellationToken cancellationToken = default);

        Task DeleteMany(FilterDefinition<TDocument> filter = null,
                        DeleteOptions options = null,
                        CancellationToken cancellationToken = default);

        Task DeleteMany(Expression<Func<TDocument, bool>> filter = null,
                        DeleteOptions options = null,
                        CancellationToken cancellationToken = default);

        Task<TDocument> DeleteOne(FilterDefinition<TDocument> filter,
                                  FindOneAndDeleteOptions<TDocument> options = null,
                                  CancellationToken cancellationToken = default);

        Task<TDocument> DeleteOne(Expression<Func<TDocument, bool>> filter = null,
                                  FindOneAndDeleteOptions<TDocument> options = null,
                                  CancellationToken cancellationToken = default);

        Task<IEnumerable<TDocument>> Find(FilterDefinition<TDocument> filter = null,
                                          FindOptions options = null,
                                          SortDefinition<TDocument> sort = null,
                                          CancellationToken cancellationToken = default);

        Task<IEnumerable<TDocument>> Find(Expression<Func<TDocument, bool>> filter = null,
                                          FindOptions options = null,
                                          SortOptions<TDocument> sort = null,
                                          CancellationToken cancellationToken = default);

        Task<PaginatedResult<TDocument>> Find(int page,
                                              int pageSize,
                                              FilterDefinition<TDocument> filter = null,
                                              FindOptions options = null,
                                              SortDefinition<TDocument> sort = null,
                                              CancellationToken cancellationToken = default);

        Task<PaginatedResult<TDocument>> Find(int page,
                                              int pageSize,
                                              Expression<Func<TDocument, bool>> filter = null,
                                              FindOptions options = null,
                                              SortOptions<TDocument> sort = null,
                                              CancellationToken cancellationToken = default);

        Task<IEnumerable<TDocument>> FindAll(SortDefinition<TDocument> sort = null,
                                             FindOptions options = null,
                                             CancellationToken cancellationToken = default);

        Task<IEnumerable<TDocument>> FindAll(SortOptions<TDocument> sort = null,
                                             FindOptions options = null,
                                             CancellationToken cancellationToken = default);

        Task<Maybe<TDocument>> FindOne(FilterDefinition<TDocument> filter,
                                       FindOptions options = null,
                                       CancellationToken cancellationToken = default);

        Task<Maybe<TDocument>> FindOne(Expression<Func<TDocument, bool>> filter,
                                       FindOptions options = null,
                                       CancellationToken cancellationToken = default);

        Task<Maybe<TDocument>> FindById(string id,
                                        FindOptions options = null,
                                        CancellationToken cancellationToken = default);

        Task<TDocument> InsertOne(TDocument document,
                                  InsertOneOptions options = null,
                                  CancellationToken cancellationToken = default);

        Task<IEnumerable<TDocument>> InsertMany(ICollection<TDocument> documents,
                                                InsertManyOptions options = null,
                                                CancellationToken cancellationToken = default);

        Task<TDocument> ReplaceOne(TDocument document,
                                   FindOneAndReplaceOptions<TDocument> options = null,
                                   CancellationToken cancellationToken = default);
    }
}