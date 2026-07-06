using Application.Modules.FriendCodes.GenerateMyCode;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/friend-codes")]
[ApiController]
[Authorize(Policy = AuthPolicies.AdminOrUser)]
public class FriendCodesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("generate-code")]
    public async Task<ActionResult<FriendCodeDTO>> GenerateMyCode()
    {
        var result = await _mediator.Send(new GenerateMyCodeCommand());

        return result.ToActionResult();
    }
}
