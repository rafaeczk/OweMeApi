using Application.Common.Interfaces;
using Application.Modules.FriendCodes.GenerateMyCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/friend-codes")]
[ApiController]
public class FriendCodesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("generate-code")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<FriendCodeDTO>> GenerateMyCode()
    {
        var result = await _mediator.Send(new GenerateMyCodeCommand());

        return result.ToActionResult();
    }
}
