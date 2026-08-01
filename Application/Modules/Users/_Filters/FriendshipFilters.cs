using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Modules.Users._Filters;

public static class FriendshipFilters
{
    public static IQueryable<T> Search<T>(this IQueryable<T> query, IUserContext user, string search)
       where T : Friendship
    {
        var searchTerm = search.Trim().ToLower();

        return query.Where(fs =>
            (user.Id == fs.UserId && (fs.Friend.FullName.ToLower().Contains(searchTerm) || fs.Friend.Email!.ToLower().Contains(searchTerm)))
            ||
            (user.Id == fs.FriendId && (fs.User.FullName.ToLower().Contains(searchTerm) || fs.User.Email!.ToLower().Contains(searchTerm))));
    }
}
