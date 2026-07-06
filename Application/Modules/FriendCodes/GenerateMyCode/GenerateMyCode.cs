using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.FriendCodes.GenerateMyCode;

public record GenerateMyCodeCommand() : IRequest<HandlerResult<FriendCodeDTO>>;

public class GenerateMyCodeHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<GenerateMyCodeCommand, HandlerResult<FriendCodeDTO>>
{
    public async Task<HandlerResult<FriendCodeDTO>> Handle(GenerateMyCodeCommand request, CancellationToken ct)
    {
        var expiredCodes = await context.FriendCodes
            .Where(c => c.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(ct);

        context.FriendCodes.RemoveRange(expiredCodes);

        var friendCode = FriendCode.ForUser(user.Id);

        context.FriendCodes.Add(friendCode);
        await context.SaveChangesAsync(ct);

        return new FriendCodeDTO(friendCode.Code, friendCode.ExpiresAt);
    }
}
