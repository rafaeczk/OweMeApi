using MediatR;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Users.Features.ChangeUserPassword;

public class ChangeUserPasswordHandler(AppDbContext context) : IRequestHandler<ChangeUserPasswordCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeUserPasswordCommand request, CancellationToken ct)
    {
        var user = await context.Users.FindAsync(request.UserId);

        if (user == null)
            return HandlerResult.Failure("User not found", ErrorCode.NotFound);

        string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        user.Hash = hash;

        await context.SaveChangesAsync(ct);

        return HandlerResult.Success();
    }
}
