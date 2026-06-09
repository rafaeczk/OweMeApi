using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Debts.Features.EditDebtInformation;

public class EditDebtInformationHandler(AppDbContext context) : IRequestHandler<EditDebtInformationCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(EditDebtInformationCommand request, CancellationToken ct)
    {
        var debt = await context.Debts.FirstOrDefaultAsync(d => d.Id == request.DebtId && d.CreditorId == request.UserId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        debt.Title = request.Title;
        debt.Description = request.Description;

        await context.SaveChangesAsync(ct);

        return HandlerResult.Success();
    }
}
