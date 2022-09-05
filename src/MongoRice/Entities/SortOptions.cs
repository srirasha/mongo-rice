using MongoDB.Driver;
using MongoRice.Extensions.Expressions;
using System;
using System.Linq.Expressions;

namespace MongoRice.Entities
{
    public class SortOptions<T>
    {
        public SortOptions(Expression<Func<T, object>> sortBy, SortDirection direction)
        {
            SortBy = sortBy;
            Direction = direction;
        }

        public SortDirection Direction { get; set; }

        public Expression<Func<T, object>> SortBy { get; set; }

        public static SortDefinition<T> BuildSortFilter(SortOptions<T> sortOptions)
        {

            return sortOptions.Direction is SortDirection.Ascending ? Builders<T>.Sort.Ascending(sortOptions.SortBy.GetFuncPropertyName()) :
                                                                      Builders<T>.Sort.Descending(sortOptions.SortBy.GetFuncPropertyName());
        }
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }
}
