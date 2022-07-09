using AutoMapper;
using CSharpFunctionalExtensions;
using MongoDB.Driver;
using MongoRice.Documents;
using MongoRice.Entities;
using System.Linq.Expressions;

namespace MongoRice.Repositories
{
    public interface IMongoRiceRepository<TEntity, TDocument>
                     where TDocument : IDocument
                     where TEntity : class, new()
    {
        IMongoCollection<TDocument> Collection { get; }

        IMapper Mapper { get; set; }

        Task DeleteById(string id,
                        FindOneAndDeleteOptions<TDocument> options = null,
                        CancellationToken cancellationToken = default);

        Task DeleteMany(FilterDefinition<TDocument> filter = null,
                        DeleteOptions options = null,
                        CancellationToken cancellationToken = default);

        Task<TDocument> DeleteOne(FilterDefinition<TDocument> filter,
                                  FindOneAndDeleteOptions<TDocument> options = null,
                                  CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> Find(FilterDefinition<TDocument> filter = null,
                                        FindOptions options = null,
                                        SortDefinition<TDocument> sort = null,
                                        CancellationToken cancellationToken = default);

        Task<PaginatedResult<TEntity>> Find(int page,
                                            int pageSize,
                                            FilterDefinition<TDocument> filter = null,
                                            FindOptions options = null,
                                            SortDefinition<TDocument> sort = null,
                                            CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> Find(Expression<Func<TDocument, bool>> filter = null,
                                        FindOptions options = null,
                                        SortDefinition<TDocument> sort = null,
                                        CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> FindAll(SortDefinition<TDocument> sort = null,
                                           FindOptions options = null,
                                           CancellationToken cancellationToken = default);

        Task<Maybe<TEntity>> FindOne(FilterDefinition<TDocument> filter,
                                     FindOptions options = null,
                                     CancellationToken cancellationToken = default);

        Task<Maybe<TEntity>> FindById(string id,
                                      FindOptions options = null,
                                      CancellationToken cancellationToken = default);

        Task<TEntity> InsertOne(TDocument document,
                                InsertOneOptions options = null,
                                CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> InsertMany(ICollection<TDocument> documents,
                                              InsertManyOptions options = null,
                                              CancellationToken cancellationToken = default);

        Task<TEntity> ReplaceOne(TDocument document,
                                 FindOneAndReplaceOptions<TDocument> options = null,
                                 CancellationToken cancellationToken = default);
    }
}