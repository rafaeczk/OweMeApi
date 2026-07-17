using Domain.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.FriendCodes.GetMyCode;

public record GetMyCodeCommand() : IRequest<Result<FriendCodeDTO>>;

public class GetMyCodeHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<GetMyCodeCommand, Result<FriendCodeDTO>>
{
    public async Task<Result<FriendCodeDTO>> Handle(GetMyCodeCommand request, CancellationToken ct)
    {
        var expiredCodes = await context.FriendCodes
            .Where(c => c.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(ct);

        context.FriendCodes.RemoveRange(expiredCodes);

        var found = await context.FriendCodes.SingleOrDefaultAsync(c => c.UserId == user.Id, ct);

        if (found is not null)
            return new FriendCodeDTO(found.Code, found.ExpiresAt);

        var friendCode = FriendCode.ForUser(user.Id);

        context.FriendCodes.Add(friendCode);
        await context.SaveChangesAsync(ct);

        return new FriendCodeDTO(friendCode.Code, friendCode.ExpiresAt);
    }
}
