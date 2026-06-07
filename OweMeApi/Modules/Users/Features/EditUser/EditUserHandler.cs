using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Data.Entities;

namespace OweMeApi.Modules.Users.Features.EditUser;

public class EditUserHandler(AppDbContext context) : IRequestHandler<EditUserCommand, Result>
{
    public async Task<Result> Handle(EditUserCommand request, CancellationToken ct)
    {
        var user = await context.Users.FindAsync([request.UserId, ct], ct);

        if (user == null)
            return Result.Failure("User not found", ErrorCode.NotFound);

        if (!(await context.UserRoles.AnyAsync(r => r.Code == request.RoleCode, ct)))
            return Result.Failure("Wrong RoleCode", ErrorCode.BadRequest);

        user.Email = request.Email;
        user.FullName = request.Fullname;
        user.RoleCode = request.RoleCode;

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}
