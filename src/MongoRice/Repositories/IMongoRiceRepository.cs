using CSharpFunctionalExtensions;
using MongoDB.Driver;
using MongoRice.Documents;
using MongoRice.Entities;
using System.Linq.Expressions;

namespace MongoRice.Repositories
{
    public interface IMongoRiceRepository<TDocument> where TDocument : IDocument
    {
        IQueryable<TDocument> AsQueryable();

        Task DeleteById(string id, CancellationToken cancellationToken = default);

        Task DeleteMany(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task DeleteOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task<IEnumerable<TDocument>> Find(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task<PaginatedResult<TDocument>> Find(FilterDefinition<TDocument> filter,
                                              SortDefinition<TDocument> sort,
                                              int page,
                                              int pageSize,
                                              CancellationToken cancellationToken = default);

        Task<Maybe<TDocument>> FindOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task<Maybe<TDocument>> FindById(string id, CancellationToken cancellationToken = default);

        Task<TDocument> InsertOne(TDocument document, CancellationToken cancellationToken = default);

        Task<ICollection<TDocument>> InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken = default);

        Task ReplaceOne(TDocument document, CancellationToken cancellationToken = default);
    }
}