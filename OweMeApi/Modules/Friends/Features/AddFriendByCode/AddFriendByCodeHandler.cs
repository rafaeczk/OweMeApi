using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Modules.Friends.Dtos;

namespace OweMeApi.Modules.Friends.Features.AddFriendByCode;

public class AddFriendByCodeHandler(AppDbContext context, FriendsService service) : IRequestHandler<AddFriendByCodeCommand, HandlerResult<AddFriendResponseDTO>>
{
    public async Task<HandlerResult<AddFriendResponseDTO>> Handle(AddFriendByCodeCommand request, CancellationToken ct)
    {
        var friendCode = await context.FriendCodes.FirstOrDefaultAsync(c => c.Code == request.Code, ct);

        if (friendCode == null || friendCode.ExpiresAt < DateTime.UtcNow)
            return HandlerResult.Failure("Code not found or expired", ErrorCode.NotFound);

        if (friendCode.UserId == request.UserId)
            return HandlerResult.Failure("You cannot enter your own code", ErrorCode.BadRequest);

        await service.CreateFriendship(request.UserId, friendCode.UserId);

        return new AddFriendResponseDTO(friendCode.UserId);
    }
}
