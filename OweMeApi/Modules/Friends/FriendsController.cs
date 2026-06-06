using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Helpers;
using OweMeApi.Modules.Friends.Dtos;

namespace OweMeApi.Modules.Friends
{
    [Route("api/friends")]
    [ApiController]
    public class FriendsController(AppDbContext context, FriendsService friendsService) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly FriendsService _friendsService = friendsService;

        [Authorize(Policy = "All")]
        [HttpGet("list")]
        public async Task<ActionResult<List<FriendListItemDTO>>> GetFriendsList()
        {
            var (userIdOk, userId) = AuthHelpers.GetUserId(User);

            if (!userIdOk)
                return BadRequest("Invalid token");

            return await _context.Friendships
                .Where(fs => (userId == fs.UserId || userId == fs.FriendId) && fs.IsAccepted)
                .Select(fs => new FriendListItemDTO(
                    fs.UserId == userId ? fs.Friend.Id : fs.User.Id,
                    fs.UserId == userId ? fs.Friend.Email : fs.User.Email,
                    fs.UserId == userId ? fs.Friend.FullName : fs.User.FullName,
                    fs.AcceptedAt ?? fs.CreatedAt
                ))
                .ToListAsync();
        }

        [Authorize(Policy = "All")]
        [HttpGet("requests")]
        public async Task<ActionResult<List<FriendRequestDTO>>> GetFriendRequestsList()
        {
            var (userIdOk, userId) = AuthHelpers.GetUserId(User);

            if (!userIdOk)
                return BadRequest("Invalid token");

            return await _context.Friendships
                .Where(fs => fs.FriendId == userId && !fs.IsAccepted)
                .Select(fs => new FriendRequestDTO(
                    fs.UserId == userId ? fs.Friend.Id : fs.User.Id,
                    fs.UserId == userId ? fs.Friend.Email : fs.User.Email,
                    fs.UserId == userId ? fs.Friend.FullName : fs.User.FullName,
                    fs.CreatedAt
                ))
                .ToListAsync();
        }

        [Authorize(Policy = "All")]
        [HttpPatch("accept-request")]
        public async Task<ActionResult<AddFriendResponseDTO>> AcceptFriendRequest([FromBody] FriendRequestActionDTO dto)
        {
            var (userIdOk, userId) = AuthHelpers.GetUserId(User);

            if (!userIdOk)
                return BadRequest("Invalid token");

            var friendship = await _context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == dto.UserId && fs.FriendId == userId);

            if (friendship == null)
                return NotFound("Friend request not found");

            friendship.IsAccepted = true;
            friendship.AcceptedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("DB error");
            }

            return Ok(new AddFriendResponseDTO(dto.UserId.ToString()));
        }

        [Authorize(Policy = "All")]
        [HttpDelete("decline-request")]
        public async Task<ActionResult> DeclineFriendRequest([FromBody] FriendRequestActionDTO dto)
        {
            var (userIdOk, userId) = AuthHelpers.GetUserId(User);

            if (!userIdOk)
                return BadRequest("Invalid token");

            var friendship = await _context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == dto.UserId && fs.FriendId == userId && !fs.IsAccepted);

            if (friendship == null)
                return NotFound("Friend request not found");

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Policy = "All")]
        [HttpPost("add-friend-by-code")]
        public async Task<ActionResult<AddFriendResponseDTO>> AddFriendByCode(AddFriendByCodeDTO dto)
        {
            var (userIdOk, userId) = AuthHelpers.GetUserId(User);

            if (!userIdOk)
                return BadRequest("Invalid token");

            var friendCode = await _context.FriendCodes.FirstOrDefaultAsync(c => c.Code == dto.Code);

            if (friendCode == null || friendCode.ExpiresAt < DateTime.UtcNow)
                return NotFound("Code not found or expired");

            if (friendCode.UserId == userId)
                return BadRequest("You cannot enter your own code");

            await _friendsService.CreateFriendship(userId, friendCode.UserId);

            return Ok(new AddFriendResponseDTO(friendCode.UserId.ToString()));
        }

        [Authorize(Policy = "All")]
        [HttpPost("request-friend-by-user-id")]
        public async Task<ActionResult<AddFriendResponseDTO>> RequestFriendByUserId(RequestFriendByUserIdDTO dto)
        {
            var (userIdOk, userId) = AuthHelpers.GetUserId(User);

            if (!userIdOk)
                return BadRequest("Invalid token");

            bool friendIdOk = Guid.TryParse(dto.FriendId, out Guid friendId);

            if (!friendIdOk)
                return BadRequest("Invalid friendId");

            await _friendsService.CreateFriendshipRequest(userId, friendId);

            return Ok(new AddFriendResponseDTO(dto.FriendId));
        }
    }
}
