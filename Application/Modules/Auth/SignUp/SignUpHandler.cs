using Application.Common;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Modules.Auth.SignUp;

public record SignUpCommand(string Email, string FullName, string Password) : IRequest<HandlerResult<Guid>>;

public class SignUpHandler(IIdentityService identityService) : IRequestHandler<SignUpCommand, HandlerResult<Guid>>
{
    public async Task<HandlerResult<Guid>> Handle(SignUpCommand request, CancellationToken ct)
    {
        var (success, user) = await identityService.SignUp(request.Email, request.Password, request.FullName);

        return success ? user.Id : HandlerResult.Failure("Sign up failure", ErrorCode.Unauthorized);
    }
}
