using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SanjeshP.Mosahebe.Data.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// اعمال فیلترهای داینامیک
        /// </summary>
        public static IQueryable<TEntity> ApplyFilters<TEntity>(
            this IQueryable<TEntity> source,
            IReadOnlyDictionary<string, string> filters,
            IReadOnlyDictionary<string, Expression<Func<TEntity, object>>> propertyMap)
            where TEntity : class
        {
            if (filters == null || filters.Count == 0 || propertyMap == null)
                return source;

            foreach (var kv in filters)
            {
                var key = kv.Key;
                var value = kv.Value?.Trim();
                if (string.IsNullOrEmpty(value) || !propertyMap.TryGetValue(key, out var propExpr))
                    continue;

                var parameter = propExpr.Parameters[0];
                var member = propExpr.Body;
                var propertyType = member.Type;

                if (propertyType == typeof(string))
                {
                    var toLowerExpr = Expression.Call(
                        member,
                        nameof(string.ToLower),
                        Type.EmptyTypes);
                    var pattern = Expression.Constant($"%{value.ToLower()}%");
                    var likeCall = Expression.Call(
                        typeof(DbFunctionsExtensions),
                        nameof(DbFunctionsExtensions.Like),
                        null,
                        Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                        toLowerExpr,
                        pattern);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(likeCall, parameter);
                    source = source.Where(lambda);
                }
                else if (propertyType == typeof(DateTime?) || propertyType == typeof(DateTime))
                {
                    if (DateTime.TryParse(value, out var dateValue))
                    {
                        var constant = Expression.Constant(dateValue.Date, typeof(DateTime?));
                        var comparison = Expression.Equal(member, constant);
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
                        source = source.Where(lambda);
                    }
                }
                else if (propertyType == typeof(Guid))
                {
                    if (Guid.TryParse(value, out var guidValue))
                    {
                        var constant = Expression.Constant(guidValue);
                        var comparison = Expression.Equal(member, constant);
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
                        source = source.Where(lambda);
                    }
                }
                else if (propertyType == typeof(bool))
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        var constant = Expression.Constant(boolValue);
                        var comparison = Expression.Equal(member, constant);
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
                        source = source.Where(lambda);
                    }
                }
                else if (propertyType.IsEnum)
                {
                    if (Enum.TryParse(propertyType, value, true, out var enumValue))
                    {
                        var constant = Expression.Constant(enumValue);
                        var comparison = Expression.Equal(member, constant);
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
                        source = source.Where(lambda);
                    }
                }
            }

            return source;
        }

        /// <summary>
        /// جستجوی عمومی روی فیلدهای رشته‌ای
        /// </summary>
        public static IQueryable<TEntity> ApplySearch<TEntity>(
            this IQueryable<TEntity> source,
            string term,
            params Expression<Func<TEntity, string>>[] props)
            where TEntity : class
        {
            if (string.IsNullOrWhiteSpace(term) || props == null || props.Length == 0)
                return source;

            term = term.Trim().ToLower();
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression orExpression = null;
            var pattern = Expression.Constant($"%{term}%");

            foreach (var prop in props)
            {
                var invoked = Expression.Invoke(prop, parameter);
                var lower = Expression.Call(
                    invoked,
                    nameof(string.ToLower),
                    Type.EmptyTypes);
                var likeCall = Expression.Call(
                    typeof(DbFunctionsExtensions),
                    nameof(DbFunctionsExtensions.Like),
                    null,
                    Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                    lower,
                    pattern);

                orExpression = orExpression == null
                    ? likeCall
                    : Expression.OrElse(orExpression, likeCall);
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(orExpression, parameter);
            return source.Where(lambda);
        }

        /// <summary>
        /// مرتب‌سازی پویا
        /// </summary>
        public static IQueryable<TEntity> ApplySorting<TEntity>(
            this IQueryable<TEntity> source,
            string sortBy,
            bool isSortDescending)
            where TEntity : class
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return source;

            var param = Expression.Parameter(typeof(TEntity), "x");
            Expression property;

            try
            {
                property = Expression.Property(param, sortBy);
            }
            catch
            {
                return source; // اگر فیلد نامعتبر باشد، بدون مرتب‌سازی ادامه بده
            }

            var lambda = Expression.Lambda(property, param);
            string methodName = isSortDescending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(TEntity), property.Type },
                source.Expression,
                Expression.Quote(lambda));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        /// <summary>
        /// صفحه‌بندی
        /// </summary>
        public static IQueryable<TEntity> ApplyPaging<TEntity>(
            this IQueryable<TEntity> source,
            int pageIndex,
            int pageSize)
            where TEntity : class
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            return source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }
    }
}