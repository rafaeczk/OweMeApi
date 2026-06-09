using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts;
using OweMeApi.Data;
using OweMeApi.Filters;

namespace OweMeApi.Modules.Debts.Features.EditDebtInformation;

public class EditDebtInformationHandler(
    AppDbContext context,
    IUserContext user) : IRequestHandler<EditDebtInformationCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(EditDebtInformationCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtCreditorOnly(user)
            .FirstOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        debt.Title = request.Title;
        debt.Description = request.Description;

        await context.SaveChangesAsync(ct);

        return HandlerResult.Success();
    }
}
