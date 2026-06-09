using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Auth.Dtos;

namespace OweMeApi.Modules.Auth.Features.SignUp;

public record SignUpCommand(string Email, string FullName, string Password) : IRequest<HandlerResult<SignUpResponseDTO>>;
