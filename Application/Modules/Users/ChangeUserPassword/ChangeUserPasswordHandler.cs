using MediatR;
using Application.Common;
using Application.Common.Interfaces;

namespace Application.Modules.Users.ChangeUserPassword;

public record ChangeUserPasswordCommand(Guid UserId, string Password) : IRequest<HandlerResult>;

public class ChangeUserPasswordHandler(IIdentityService identityService) : IRequestHandler<ChangeUserPasswordCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeUserPasswordCommand request, CancellationToken ct)
    {
        var (userFound, _) = await identityService.GetUserById(request.UserId);

        if (!userFound)
            return HandlerResult.Failure("User not found", ErrorCode.NotFound);

        bool success = await identityService.ResetPassword(request.UserId, request.Password);

        return success ? HandlerResult.Success() : HandlerResult.Failure("Reset password error", ErrorCode.BadRequest);
    }
}
