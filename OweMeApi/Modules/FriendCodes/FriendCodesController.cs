using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OweMeApi.Extensions;
using OweMeApi.Modules.FriendCodes.Features.GenerateMyCode;

namespace OweMeApi.Modules.FriendCodes;

[Route("api/friend-codes")]
[ApiController]
public class FriendCodesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("generate-code")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<FriendCodeDTO>> GenerateMyCode()
    {
        var result = await _mediator.Send(new GenerateMyCodeCommand(User.GetUserId()));

        return result.ToActionResult();
    }
}
