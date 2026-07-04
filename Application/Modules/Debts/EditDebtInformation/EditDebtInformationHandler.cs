using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;

namespace Application.Modules.Debts.EditDebtInformation;

public record EditDebtInformationCommand(Guid DebtId, string Title, string? Description) : IRequest<HandlerResult>;

public class EditDebtInformationHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<EditDebtInformationCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(EditDebtInformationCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtCreditorOnly(user)
            .SingleOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        debt.UpdateProfile(request.Title, request.Description);

        await context.SaveChangesAsync(ct);

        return HandlerResult.Success();
    }
}
