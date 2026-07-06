using Application.Common;
using Application.Common.Interfaces;
using Application.Modules.Friends._Shared;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Friends.AddFriendByCode;

public record AddFriendByCodeCommand(string Code) : IRequest<HandlerResult<AddFriendResponseDTO>>;

public class AddFriendByCodeHandler(
    IAppDbContext context, 
    IUserContext user) : IRequestHandler<AddFriendByCodeCommand, HandlerResult<AddFriendResponseDTO>>
{
    public async Task<HandlerResult<AddFriendResponseDTO>> Handle(AddFriendByCodeCommand request, CancellationToken ct)
    {
        var friendCode = await context.FriendCodes.FirstOrDefaultAsync(c => c.Code == request.Code, ct);

        if(friendCode == null)
            return HandlerResult.Failure("Friend code not found", ErrorCode.NotFound);

        try
        {
            var friendship = Friendship.FromFriendCode(user.Id, friendCode);

            context.Friendships.Add(friendship);
            await context.SaveChangesAsync(ct);
        }
        catch(FriendCodeExpiredException e)
        {
            return HandlerResult.Failure(e.Message, ErrorCode.NotFound);
        }
        catch(YourFriendCodeException e)
        {
            return HandlerResult.Failure(e.Message, ErrorCode.BadRequest);
        }

        return new AddFriendResponseDTO(friendCode.UserId);
    }
}
