using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Debts.Features.GetDebts;

public enum QEUserRoleInDebt
{
    Debtor, Creditor, Any
}

public enum QEDebtState
{
    Settled, Unsettled, Any
}

public record GetDebtsQuery(Guid UserId, QEUserRoleInDebt Role, QEDebtState State) : IRequest<HandlerResult<List<DebtListItemDTO>>>;
