using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Auth.Features.SignIn;

public class SignInHandler(AppDbContext context, AuthService service) : IRequestHandler<SignInCommand, HandlerResult<string>>
{
    public async Task<HandlerResult<string>> Handle(SignInCommand request, CancellationToken ct)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Hash))
            return HandlerResult.Failure("Wrong email or password", ErrorCode.BadRequest);

        var token = service.CreateToken(user);

        return token;
    }
}
