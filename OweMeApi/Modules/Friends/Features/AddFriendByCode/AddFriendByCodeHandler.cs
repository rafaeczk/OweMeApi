using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts;
using OweMeApi.Data;

namespace OweMeApi.Modules.Friends.Features.AddFriendByCode;

public class AddFriendByCodeHandler(
    AppDbContext context, 
    FriendsService service,
    IUserContext user) : IRequestHandler<AddFriendByCodeCommand, HandlerResult<AddFriendResponseDTO>>
{
    public async Task<HandlerResult<AddFriendResponseDTO>> Handle(AddFriendByCodeCommand request, CancellationToken ct)
    {
        var friendCode = await context.FriendCodes.FirstOrDefaultAsync(c => c.Code == request.Code, ct);

        if (friendCode == null || friendCode.ExpiresAt < DateTime.UtcNow)
            return HandlerResult.Failure("Code not found or expired", ErrorCode.NotFound);

        if (friendCode.UserId == user.Id)
            return HandlerResult.Failure("You cannot enter your own code", ErrorCode.BadRequest);

        await service.CreateFriendship(user.Id, friendCode.UserId);

        return new AddFriendResponseDTO(friendCode.UserId);
    }
}
