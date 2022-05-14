using CSharpFunctionalExtensions;
using MongoRice.Documents;
using System.Linq.Expressions;

namespace MongoRice.Repositories
{
    public interface IMongoRiceRepository<TDocument> where TDocument : IDocument
    {
        IQueryable<TDocument> AsQueryable();

        Task DeleteById(string id, CancellationToken cancellationToken = default);

        Task DeleteMany(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

        Task DeleteOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

        Task<IEnumerable<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

        Task<Maybe<TDocument>> FindOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

        Task<Maybe<TDocument>> FindById(string id, CancellationToken cancellationToken = default);

        Task<TDocument> InsertOne(TDocument document, CancellationToken cancellationToken = default);

        Task<ICollection<TDocument>> InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken = default);

        Task ReplaceOne(TDocument document, CancellationToken cancellationToken = default);
    }
}