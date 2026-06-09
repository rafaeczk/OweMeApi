using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Data.Entities;
using OweMeApi.Modules.Auth.Dtos;

namespace OweMeApi.Modules.Auth.Features.SignUp;

public class SignUpHandler(AppDbContext context) : IRequestHandler<SignUpCommand, HandlerResult<SignUpResponseDTO>>
{
    public async Task<HandlerResult<SignUpResponseDTO>> Handle(SignUpCommand request, CancellationToken ct)
    {
        if (await context.Users.AnyAsync(u => u.Email == request.Email, ct))
            return HandlerResult.Failure("This email is already in use.", ErrorCode.BadRequest);

        string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName,
            Hash = hash
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(ct);

        return new SignUpResponseDTO(user.Id);
    }
}
