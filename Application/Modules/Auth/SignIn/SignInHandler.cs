using Application.Common;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Modules.Auth.SignIn;

public record SignInCommand(string Email, string Password) : IRequest<HandlerResult>;

public class SignInHandler(IIdentityService identityService) : IRequestHandler<SignInCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(SignInCommand request, CancellationToken ct)
    {
        var success = await identityService.SignIn(request.Email, request.Password);

        return success ? HandlerResult.Success() : HandlerResult.Failure("Sign in failure", ErrorCode.Unauthorized);
    }
}
