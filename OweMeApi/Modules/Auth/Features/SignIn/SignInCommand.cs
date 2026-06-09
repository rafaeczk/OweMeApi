using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Auth.Features.SignIn;

public record SignInCommand(string Email, string Password) : IRequest<HandlerResult<string>>;
