using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Auth.Features.SignUp;

public record SignUpCommand(string Email, string FullName, string Password) : IRequest<HandlerResult<SignUpResponseDTO>>;
