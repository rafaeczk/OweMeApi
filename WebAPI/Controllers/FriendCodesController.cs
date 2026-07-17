using Application.Common.Extensions;
using Application.Modules.FriendCodes.GetMyCode;
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

    [HttpGet("get-my-code")]
    public async Task<ActionResult<FriendCodeDTO>> GetMyCode()
    {
        var result = await _mediator.Send(new GetMyCodeCommand());

        return result.ToActionResult();
    }
}
