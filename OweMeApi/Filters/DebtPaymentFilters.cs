using OweMeApi.Contexts;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Filters;

public static class DebtPaymentFilters
{
    public static IQueryable<T> DebtPaymentPayerOnly<T>(this IQueryable<T> query, IUserContext user)
        where T : DebtPayment
    {
        switch (user.Role)
        {
            case "ADMIN":
                return query;
            case "USER":
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
            case "ADMIN":
                return query;
            case "USER":
                return query.Where(p => p.ReceiverId == user.Id);
            default:
                break;
        }

        return query.Where(d => false);
    }
}
