using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;

namespace Application.Modules.Debts.EditDebtInformation;

public record EditDebtInformationCommand(Guid DebtId, string Title, string? Description) : IRequest<Result>;

public class EditDebtInformationHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<EditDebtInformationCommand, Result>
{
    public async Task<Result> Handle(EditDebtInformationCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtCreditorOnly(user)
            .SingleOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt is null)
            return Result.Failure("Debt not found", FailureReason.NotFound);

        debt.UpdateProfile(request.Title, request.Description);

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}
