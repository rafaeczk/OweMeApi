using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Data.Entities;
using OweMeApi.Helpers;
using OweMeApi.Modules.FriendCodes.Dtos;

namespace OweMeApi.Modules.FriendCodes;

[Route("api/friend-codes")]
[ApiController]
public class FriendCodesController(AppDbContext context, FriendCodesService friendCodesService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly FriendCodesService _friendCodesService = friendCodesService;

    [HttpGet("generate-code")]
    [Authorize(Roles = "ADMIN,MODERATOR,USER")]
    public async Task<ActionResult<FriendCodeDTO>> GenerateMyCode()
    {
        await _friendCodesService.DeleteExpiredCodes();

        var (userIdOk, userId) = AuthHelpers.GetUserId(User);

        if (!userIdOk)
            return BadRequest("Invalid token");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found");

        var friendCode = new FriendCode()
        {
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(3),
            Code = _friendCodesService.GenerateFriendCode()
        };

        _context.FriendCodes.Add(friendCode);
        await _context.SaveChangesAsync();

        return Ok(new FriendCodeDTO(friendCode.Code, friendCode.ExpiresAt));
    }
}
