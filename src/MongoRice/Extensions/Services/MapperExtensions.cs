using AutoMapper;
using System.Linq.Expressions;
using System.Reflection;

namespace MongoRice.Extensions.Services
{
    public static class MapperExtensions
    {
        public static Expression<Func<TEntity, bool>> ConvertPredicate<TDto, TEntity>(this IMapper mapper, Expression<Func<TDto, bool>> predicate)
        {
            return (Expression<Func<TEntity, bool>>)new PredicateVisitor<TDto, TEntity>(mapper).Visit(predicate);
        }

        public class PredicateVisitor<TDto, TEntity> : ExpressionVisitor
        {
            private readonly ParameterExpression _entityParameter;
            private readonly MemberAssignment[] _bindings;

            public PredicateVisitor(IMapper mapper)
            {
                IQueryable<TDto> mockQuery = mapper.ProjectTo<TDto>(new TEntity[0].AsQueryable(), null);
                LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)((MethodCallExpression)mockQuery.Expression).Arguments[1]).Operand;

                _bindings = ((MemberInitExpression)lambdaExpression.Body).Bindings.Cast<MemberAssignment>().ToArray();
                _entityParameter = Expression.Parameter(typeof(TEntity));
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                return Expression.Lambda(
                    base.Visit(node.Body),
                    node.Parameters.Select(p => (ParameterExpression)base.Visit(p)).ToArray()
                );
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                MemberInfo member = node.Member;
                MemberAssignment binding = _bindings.FirstOrDefault(b => b.Member == member);

                if (binding != null)
                {
                    return base.Visit(binding.Expression);
                }

                return base.VisitMember(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node.Type == typeof(TDto))
                {
                    return _entityParameter;
                }
                if (node.Type == typeof(TEntity))
                {
                    return _entityParameter;
                }

                return base.VisitParameter(node);
            }
        }
    }
}