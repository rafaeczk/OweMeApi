using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Debts.Features.GetDebt;

public class GetDebtHandler(AppDbContext context, DebtsService service) : IRequestHandler<GetDebtQuery, HandlerResult<DebtDTO>>
{
    public async Task<HandlerResult<DebtDTO>> Handle(GetDebtQuery request, CancellationToken ct)
    {
        var debt = await context.Debts
            .FirstOrDefaultAsync(d => d.Id == request.DebtId && (d.CreditorId == request.UserId || d.DebtorId == request.UserId), ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        var totalAmount = await service.GetTotalAmount(debt.Id, ct);

        var totalPayments = await service.GetTotalPayments(debt.Id, ct);

        var summary = service.GetSummary(totalAmount, totalPayments, debt.CreditorId, debt.DebtorId);

        var creditorApproves = await service.GetCreditorApproves(debt.Id, ct);

        var debtorApproves = await service.GetDebtorApproves(debt.Id, ct);

        var debtIsSettled = await service.GetDebtIsSettled(debt.Id, ct);

        return new DebtDTO(
            debt.Id,
            debt.Title,
            debt.Description,
            debt.CreditorId,
            debt.DebtorId,
            totalAmount,
            totalPayments,
            summary,
            creditorApproves,
            debtorApproves,
            debtIsSettled
        );
    }
}
