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

        Task DeleteMany(FilterDefinition<TDocument> filter = null, CancellationToken cancellationToken = default);

        Task<TDocument> DeleteOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> Find(FilterDefinition<TDocument> filter = null,
                                        SortDefinition<TDocument> sort = null,
                                        CancellationToken cancellationToken = default);

        Task<PaginatedResult<TEntity>> Find(int page,
                                            int pageSize,
                                            FilterDefinition<TDocument> filter = null,
                                            SortDefinition<TDocument> sort = null,
                                            CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> Find(Expression<Func<TDocument, bool>> filter = null,
                                        SortDefinition<TDocument> sort = null,
                                        CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> FindAll(SortDefinition<TDocument> sort = null,
                                           CancellationToken cancellationToken = default);

        Task<Maybe<TEntity>> FindOne(FilterDefinition<TDocument> filter,
                                     CancellationToken cancellationToken = default);

        Task<Maybe<TEntity>> FindById(string id,
                                      CancellationToken cancellationToken = default);

        Task<TEntity> InsertOne(TDocument document,
                                CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> InsertMany(ICollection<TDocument> documents,
                                   CancellationToken cancellationToken = default);

        Task<TEntity> ReplaceOne(TDocument document,
                                 CancellationToken cancellationToken = default);
    }
}