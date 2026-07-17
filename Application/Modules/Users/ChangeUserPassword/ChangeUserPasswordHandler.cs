using MediatR;
using Domain.Common;
using Application.Common.Interfaces;

namespace Application.Modules.Users.ChangeUserPassword;

public record ChangeUserPasswordCommand(Guid UserId, string Password) : IRequest<Result>;

public class ChangeUserPasswordHandler(IIdentityService identityService) : IRequestHandler<ChangeUserPasswordCommand, Result>
{
    public async Task<Result> Handle(ChangeUserPasswordCommand request, CancellationToken ct)
    {
        var result = await identityService.ResetPassword(request.UserId, request.Password);

        if (!result.IsSuccess)
            return Result.Failure(result.Errors, FailureReason.BadRequest);

        return Result.Success();
    }
}
