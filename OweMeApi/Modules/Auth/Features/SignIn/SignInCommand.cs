using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Auth.Dtos;

namespace OweMeApi.Modules.Auth.Features.SignIn;

public record SignInCommand(string Email, string Password) : IRequest<HandlerResult<string>>;
