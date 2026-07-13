using Domain.Common;
using System.Linq.Expressions;

namespace Application.Common.Pagination;

public static class PaginationExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PaginationParams pagination)
        where T : BaseAuditableEntity
    {
        return query
            .OrderBy(x => x.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PaginationParams pagination, Expression<Func<T, object>> orderBy)
    {
        return query
            .OrderBy(orderBy)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }
}
