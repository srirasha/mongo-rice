using System.Linq.Expressions;
using System.Reflection;

namespace MongoRice.Extensions.Expressions
{
    public static class ExpressionsExtensions
    {
        public static string GetFuncPropertyName<T>(this Expression<Func<T, object>> func)
        {
            MemberExpression memberExpression;

            if (func.Body is UnaryExpression unaryExpression)
            {
                memberExpression = (MemberExpression)(unaryExpression.Operand);
            }
            else
            {
                memberExpression = (MemberExpression)(func.Body);
            }

            return ((PropertyInfo)memberExpression.Member).Name;
        }
    }
}