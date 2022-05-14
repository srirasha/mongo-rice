using CSharpFunctionalExtensions;
using Library.Documents;
using System.Linq.Expressions;

namespace Library.Repositories
{
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        IQueryable<TDocument> AsQueryable();

        Task DeleteById(string id, CancellationToken cancellationToken);

        Task DeleteMany(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);

        Task DeleteOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);

        Task<IEnumerable<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);

        Task<Maybe<TDocument>> FindOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);

        Task<Maybe<TDocument>> FindById(string id, CancellationToken cancellationToken);

        Task<TDocument> InsertOne(TDocument document, CancellationToken cancellationToken);

        Task<ICollection<TDocument>> InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken);

        Task ReplaceOne(TDocument document, CancellationToken cancellationToken);
    }
}