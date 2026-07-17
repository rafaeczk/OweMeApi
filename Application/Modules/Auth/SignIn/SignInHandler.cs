using Application.Common.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Modules.Auth.SignIn;

public record SignInCommand(string Email, string Password) : IRequest<Result<string>>;

public class SignInHandler(IIdentityService identityService) : IRequestHandler<SignInCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SignInCommand request, CancellationToken ct)
    {
        var result = await identityService.SignIn(request.Email, request.Password);

        if (!result.IsSuccess)
            return Result.Failure(result.Errors, FailureReason.Unauthorized);

        return result.Value!;
    }
}
