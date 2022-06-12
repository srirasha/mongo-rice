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
        Task DeleteById(string id, CancellationToken cancellationToken = default);

        Task DeleteMany(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task DeleteOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> Find(FilterDefinition<TDocument> filter,
                                        SortDefinition<TDocument> sort = null,
                                        CancellationToken cancellationToken = default);

        Task<PaginatedResult<TEntity>> Find(FilterDefinition<TDocument> filter,
                                            SortDefinition<TDocument> sort,
                                            int page,
                                            int pageSize,
                                            CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> Find(Expression<Func<TDocument, bool>> filter, SortDefinition<TDocument> sort = null, CancellationToken cancellationToken = default);

        Task<Maybe<TEntity>> FindOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task<Maybe<TEntity>> FindById(string id, CancellationToken cancellationToken = default);

        Task<TEntity> InsertOne(TDocument document, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken = default);

        Task<TEntity> ReplaceOne(TDocument document, CancellationToken cancellationToken = default);
    }
}