using System;
using System.Linq;
using System.Linq.Expressions;

namespace Business.Utilities.Predicate
{
    static class QuerySortPredicate
    {
        internal static IQueryable<T> WhereMethod<T>(this IQueryable<T> source, string _method, string memberPath, string value)
        {
            var parameter = Expression.Parameter(typeof(T), typeof(T).Name);
            var property = typeof(T).GetProperty(memberPath);
            var propertyType = property.PropertyType;
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var method = typeof(string).GetMethod(_method, new[] { typeof(string) });
            var someValue = Expression.Constant(value);
            var propertyValue = Expression.Convert(propertyAccess, typeof(object)).Operand;
            MethodCallExpression containsMethodExp = null;
            if (propertyType != typeof(string))
            {
                var convertedExpression = Expression.Call(propertyValue, typeof(object).GetMethod("ToString"));
                containsMethodExp = Expression.Call(convertedExpression, method, someValue);
            }
            else
            {
                containsMethodExp = Expression.Call(propertyValue, method, someValue);
            }

            var predicate = Expression.Lambda<Func<T, bool>>(containsMethodExp, parameter);
            return source.Where(predicate);
        }
    }
}
