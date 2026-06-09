using MediatR;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Data.Entities;

namespace OweMeApi.Modules.FriendCodes.Features.GenerateMyCode;

public class GenerateMyCodeHandler(
    AppDbContext context,
    FriendCodesService service) : IRequestHandler<GenerateMyCodeCommand, HandlerResult<FriendCodeDTO>>
{
    public async Task<HandlerResult<FriendCodeDTO>> Handle(GenerateMyCodeCommand request, CancellationToken ct)
    {
        await service.DeleteExpiredCodes();

        var friendCode = new FriendCode()
        {
            UserId = request.UserId,
            ExpiresAt = DateTime.UtcNow.AddHours(12),
            Code = service.GenerateFriendCode()
        };

        context.FriendCodes.Add(friendCode);
        await context.SaveChangesAsync(ct);

        return new FriendCodeDTO(friendCode.Code, friendCode.ExpiresAt);
    }
}
