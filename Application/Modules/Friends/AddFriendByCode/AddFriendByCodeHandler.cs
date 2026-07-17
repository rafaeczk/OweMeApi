using Application.Common.Interfaces;
using Application.Modules.Friends._Shared;
using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Friends.AddFriendByCode;

public record AddFriendByCodeCommand(string Code) : IRequest<Result<AddFriendResponseDTO>>;

public class AddFriendByCodeHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<AddFriendByCodeCommand, Result<AddFriendResponseDTO>>
{
    public async Task<Result<AddFriendResponseDTO>> Handle(AddFriendByCodeCommand request, CancellationToken ct)
    {
        var friendCode = await context.FriendCodes.FirstOrDefaultAsync(c => c.Code == request.Code, ct);

        if (friendCode is null)
            return Result.Failure("Friend code not found", FailureReason.NotFound);

        if (await context.Friendships.AnyAsync(f => (f.UserId == user.Id && f.FriendId == friendCode.UserId) || (f.UserId == friendCode.UserId && f.FriendId == user.Id), ct))
            return Result.Failure("Cannot pass your friend code", FailureReason.BadRequest);

        try
        {
            var friendship = Friendship.FromFriendCode(user.Id, friendCode);

            context.Friendships.Add(friendship);
            await context.SaveChangesAsync(ct);
        }
        catch (FriendCodeExpiredException e)
        {
            return Result.Failure(e.Message, FailureReason.NotFound);
        }
        catch (YourFriendCodeException e)
        {
            return Result.Failure(e.Message, FailureReason.BadRequest);
        }

        return new AddFriendResponseDTO(friendCode.UserId);
    }
}
