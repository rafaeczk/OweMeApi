using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OweMeApi.Data;
using OweMeApi.Data.Entities;
using OweMeApi.Extensions;
using OweMeApi.Modules.FriendCodes.Dtos;

namespace OweMeApi.Modules.FriendCodes;

[Route("api/friend-codes")]
[ApiController]
public class FriendCodesController(AppDbContext context, FriendCodesService friendCodesService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly FriendCodesService _friendCodesService = friendCodesService;

    [HttpGet("generate-code")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<FriendCodeDTO>> GenerateMyCode()
    {
        await _friendCodesService.DeleteExpiredCodes();

        var userId = User.GetUserId();

        var friendCode = new FriendCode()
        {
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddHours(12),
            Code = _friendCodesService.GenerateFriendCode()
        };

        _context.FriendCodes.Add(friendCode);
        await _context.SaveChangesAsync();

        return Ok(new FriendCodeDTO(friendCode.Code, friendCode.ExpiresAt));
    }
}
