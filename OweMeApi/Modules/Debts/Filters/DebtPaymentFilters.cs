using OweMeApi.Contexts.IUserContext;
using OweMeApi.Data.Entities.Ledger;
using OweMeApi.Modules.Users.Domain.Enums;

namespace OweMeApi.Modules.Debts.Filters;

public static class DebtPaymentFilters
{
    public static IQueryable<T> DebtPaymentPayerOnly<T>(this IQueryable<T> query, IUserContext user)
        where T : DebtPayment
    {
        switch (user.Role)
        {
            case UserRole.Admin:
                return query;
            case UserRole.User:
                return query.Where(p => p.PayerId == user.Id);
            default:
                break;
        }

        return query.Where(d => false);
    }

    public static IQueryable<T> DebtPaymentReceiverOnly<T>(this IQueryable<T> query, IUserContext user)
        where T : DebtPayment
    {
        switch (user.Role)
        {
            case UserRole.Admin:
                return query;
            case UserRole.User:
                return query.Where(p => p.ReceiverId == user.Id);
            default:
                break;
        }

        return query.Where(d => false);
    }
}
