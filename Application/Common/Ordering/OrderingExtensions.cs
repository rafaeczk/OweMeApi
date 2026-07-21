using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Common.Ordering;

public static class OrderingExtensions
{
    public static IQueryable<Debt> Order(this IQueryable<Debt> query, OrderingParams ordering)
    {
        return query.ApplyDynamicOrder(
            ordering.OrderBy,
            ordering.OrderDesc,
            new Dictionary<string, Expression<Func<Debt, object?>>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Title", d => d.Title },
                { "Description", d => d.Description },
                { "CreatedAt", d => d.CreatedAt },
                { "UpdatedAt", d => d.UpdatedAt }
            },
            d => d.CreatedAt
        );
    }

    public static IQueryable<Friendship> Order(this IQueryable<Friendship> query, OrderingParams ordering)
    {
        return query.ApplyDynamicOrder(
            ordering.OrderBy,
            ordering.OrderDesc,
            new Dictionary<string, Expression<Func<Friendship, object?>>>(StringComparer.OrdinalIgnoreCase)
            {
                { "AcceptedAt", f => f.AcceptedAt },
                { "Since", f => f.AcceptedAt },
                { "CreatedAt", f => f.CreatedAt },
                { "RequestedAt", f => f.CreatedAt }
            },
            f => f.CreatedAt
        );
    }

    public static IQueryable<User> Order(this IQueryable<User> query, OrderingParams ordering)
    {
        return query.ApplyDynamicOrder(
            ordering.OrderBy,
            ordering.OrderDesc,
            new Dictionary<string, Expression<Func<User, object?>>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", u => u.Id },
                { "Email", u => u.Email },
                { "FullName", u => u.FullName }
            },
            u => u.Id
        );
    }

    // PRIVATE

    private static IQueryable<T> ApplyDynamicOrder<T>(
        this IQueryable<T> query,
        string? orderBy,
        bool orderDesc,
        Dictionary<string, Expression<Func<T, object?>>> selectors,
        Expression<Func<T, object>> defaultSelector)
    {
        if (string.IsNullOrWhiteSpace(orderBy) || !selectors.TryGetValue(orderBy.Trim(), out var selector))
            return query.OrderBy(defaultSelector);

        Expression body = selector.Body;
        if (body is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
            body = unaryExpression.Operand;

        var lambda = Expression.Lambda(body, selector.Parameters);

        string methodName = orderDesc ? "OrderByDescending" : "OrderBy";

        var methodCall = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), body.Type],
            query.Expression,
            Expression.Quote(lambda)
        );

        return query.Provider.CreateQuery<T>(methodCall);
    }
}
