using Application.Common.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Modules.Auth.SignUp;

public record SignUpCommand(string Email, string FullName, string Password) : IRequest<Result<Guid>>;

public class SignUpHandler(IIdentityService identityService) : IRequestHandler<SignUpCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SignUpCommand request, CancellationToken ct)
    {
        var result = await identityService.SignUp(request.Email, request.Password, request.FullName);

        if (!result.IsSuccess)
            return Result.Failure(result.Errors, FailureReason.Unauthorized);

        return result.Value!.Id;
    }
}
