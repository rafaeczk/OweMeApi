using MediatR;
using Application.Common;
using Application.Common.Interfaces;

namespace Application.Modules.Users.ChangeUserPassword;

public record ChangeUserPasswordCommand(Guid UserId, string Password) : IRequest<HandlerResult>;

public class ChangeUserPasswordHandler(IIdentityService identityService) : IRequestHandler<ChangeUserPasswordCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeUserPasswordCommand request, CancellationToken ct)
    {
        var result = await identityService.ResetPassword(request.UserId, request.Password);

        if (!result.IsSuccess)
            return HandlerResult.Failure(result.Errors, ErrorCode.BadRequest);

        return HandlerResult.Success();
    }
}
