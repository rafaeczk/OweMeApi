using Application.Common;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Modules.Auth.SignIn;

public record SignInCommand(string Email, string Password) : IRequest<HandlerResult>;

public class SignInHandler(IIdentityService identityService) : IRequestHandler<SignInCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(SignInCommand request, CancellationToken ct)
    {
        var result = await identityService.SignIn(request.Email, request.Password);

        if (!result.IsSuccess)
            return HandlerResult.Failure(result.Errors, ErrorCode.Unauthorized);

        return HandlerResult.Success();
    }
}
