using OweMeApi.Contexts.IUserContext;
using OweMeApi.Data.Entities.Ledger;
using OweMeApi.Modules.Debts.Domain.Enums;
using OweMeApi.Modules.Users.Domain.Enums;

namespace OweMeApi.Modules.Debts.Filters;

public static class DebtFilters
{
    public static IQueryable<T> DebtOwnerOnly<T>(this IQueryable<T> query, IUserContext user)
        where T : Debt
    {
        switch (user.Role)
        {
            case SystemUserRole.Admin:
                return query;
            case SystemUserRole.User:
                return query.Where(d => d.CreditorId == user.Id || d.DebtorId == user.Id);
            default:
                break;
        }

        return query.Where(d => false);
    }

    public static IQueryable<T> DebtCreditorOnly<T>(this IQueryable<T> query, IUserContext user)
        where T : Debt
    {
        switch (user.Role)
        {
            case SystemUserRole.Admin:
                return query;
            case SystemUserRole.User:
                return query.Where(d => d.CreditorId == user.Id);
            default:
                break;
        }

        return query.Where(d => false);
    }

    public static IQueryable<T> DebtDebtorOnly<T>(this IQueryable<T> query, IUserContext user)
        where T : Debt
    {
        switch (user.Role)
        {
            case SystemUserRole.Admin:
                return query;
            case SystemUserRole.User:
                return query.Where(d => d.DebtorId == user.Id);
            default:
                break;
        }

        return query.Where(d => false);
    }

    public static IQueryable<T> DebtUnsettled<T>(this IQueryable<T> query)
        where T : Debt => 
        query.Where(d => !d.LedgerEvents.Any(e => e.EventType == LedgerEventType.DebtSettlement));
}
