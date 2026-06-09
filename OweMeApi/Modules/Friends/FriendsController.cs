using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OweMeApi.Data;
using OweMeApi.Extensions;
using OweMeApi.Modules.Friends.Features;
using OweMeApi.Modules.Friends.Features.AcceptFriendRequest;
using OweMeApi.Modules.Friends.Features.AddFriendByCode;
using OweMeApi.Modules.Friends.Features.DeclineFriendRequest;
using OweMeApi.Modules.Friends.Features.GetFriendRequestsList;
using OweMeApi.Modules.Friends.Features.GetFriendsList;
using OweMeApi.Modules.Friends.Features.RequestFriendByUserId;

namespace OweMeApi.Modules.Friends
{
    [Route("api/friends")]
    [ApiController]
    public class FriendsController(AppDbContext context, FriendsService friendsService, IMediator mediator) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly FriendsService _friendsService = friendsService;
        private readonly IMediator _mediator = mediator;

        [Authorize(Policy = "All")]
        [HttpGet("list")]
        public async Task<ActionResult<List<FriendListItemDTO>>> GetFriendsList()
        {
            var result = await _mediator.Send(new GetFriendsListQuery(User.GetUserId()));

            return result.ToActionResult();
        }

        [Authorize(Policy = "All")]
        [HttpGet("requests")]
        public async Task<ActionResult<List<FriendRequestDTO>>> GetFriendRequestsList()
        {
            var result = await _mediator.Send(new GetFriendRequestsListQuery(User.GetUserId()));

            return result.ToActionResult();
        }

        [Authorize(Policy = "All")]
        [HttpPatch("accept-request")]
        public async Task<ActionResult> AcceptFriendRequest([FromBody] FriendRequestActionDTO dto)
        {
            var result = await _mediator.Send(new AcceptFriendRequestCommand(User.GetUserId(), dto.FriendId));

            return result.ToActionResult();
        }

        [Authorize(Policy = "All")]
        [HttpDelete("decline-request")]
        public async Task<ActionResult> DeclineFriendRequest([FromBody] FriendRequestActionDTO dto)
        {
            var result = await _mediator.Send(new DeclineFriendRequestCommand(User.GetUserId(), dto.FriendId));

            return result.ToActionResult();
        }

        [Authorize(Policy = "All")]
        [HttpPost("add-friend-by-code")]
        public async Task<ActionResult<AddFriendResponseDTO>> AddFriendByCode(AddFriendByCodeDTO dto)
        {
            var result = await _mediator.Send(new AddFriendByCodeCommand(User.GetUserId(), dto.Code));

            return result.ToActionResult();
        }

        [Authorize(Policy = "All")]
        [HttpPost("request-friend-by-user-id")]
        public async Task<ActionResult<AddFriendResponseDTO>> RequestFriendByUserId(RequestFriendByUserIdDTO dto)
        {
            var result = await _mediator.Send(new RequestFriendByUserIdCommand(User.GetUserId(), dto.FriendId));

            return result.ToActionResult();
        }
    }
}
