using Application.Common;
using Application.Common.Interfaces;
using Application.Modules.Auth.SignIn;
using MediatR;

namespace Application.Modules.Auth.LogOut;

public record LogOutCommand : IRequest<HandlerResult>;

public class LogOutHandler(IIdentityService identityService) : IRequestHandler<LogOutCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(LogOutCommand request, CancellationToken ct)
    {
        await identityService.LogOut();
        return HandlerResult.Success();
    }
}
