using Application.Common.Interfaces;
using Application.Modules.Debts.ChangeDebtAmount;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Modules.FriendCodes.GetMyCode;

public record GetMyCodeCommand() : IRequest<Result<FriendCodeDTO>>;

public class GetMyCodeHandler(
    IAppDbContext context,
    IUserContext user, 
    ILogger<ChangeDebtAmountHandler> logger) : IRequestHandler<GetMyCodeCommand, Result<FriendCodeDTO>>
{
    public async Task<Result<FriendCodeDTO>> Handle(GetMyCodeCommand request, CancellationToken ct)
    {
        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            await context.FriendCodes
                .Where(c => c.ExpiresAt < DateTime.UtcNow)
                .ExecuteDeleteAsync(ct);

            var found = await context.FriendCodes
                .SingleOrDefaultAsync(c => c.UserId == user.Id, ct);

            if (found is not null)
            {
                await transaction.CommitAsync(ct);
                return new FriendCodeDTO(found.Code, found.ExpiresAt);
            }

            var friendCode = FriendCode.ForUser(user.Id);
            context.FriendCodes.Add(friendCode);
            await context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
            return new FriendCodeDTO(friendCode.Code, friendCode.ExpiresAt);
        }
        catch (Exception exception)
        {
            try
            {
                await transaction.RollbackAsync(ct);
            }
            catch { }

            logger.LogError(exception, "Getting friend code error: UserId={UserId}", user.Id);

            return Result.Failure("Technical error", FailureReason.InternalError);
        }
    }
}
