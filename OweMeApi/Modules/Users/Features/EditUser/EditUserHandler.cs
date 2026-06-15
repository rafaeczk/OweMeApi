using MediatR;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Modules.Users.Domain.Enums;

namespace OweMeApi.Modules.Users.Features.EditUser;

public class EditUserHandler(AppDbContext context) : IRequestHandler<EditUserCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(EditUserCommand request, CancellationToken ct)
    {
        var user = await context.Users.FindAsync([request.UserId, ct], ct);

        if (user == null)
            return HandlerResult.Failure("User not found", ErrorCode.NotFound);

        if (!UserRole.Verify(request.RoleCode))
            return HandlerResult.Failure("Wrong RoleCode", ErrorCode.BadRequest);

        user.Email = request.Email;
        user.FullName = request.FullName;
        user.RoleCode = request.RoleCode;

        await context.SaveChangesAsync(ct);

        return HandlerResult.Success();
    }
}
