using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OweMeApi.Modules.Friends.Features.AcceptFriendRequest;
using OweMeApi.Modules.Friends.Features.AddFriendByCode;
using OweMeApi.Modules.Friends.Features.DeclineFriendRequest;
using OweMeApi.Modules.Friends.Features.GetFriendRequestsList;
using OweMeApi.Modules.Friends.Features.GetFriendsList;
using OweMeApi.Modules.Friends.Features.RequestFriendByUserId;
using OweMeApi.Modules.Friends.Features._Shared;

namespace OweMeApi.Modules.Friends
{
    [Route("api/friends")]
    [ApiController]
    public class FriendsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Policy = "User")]
        [HttpGet("list")]
        public async Task<ActionResult<List<FriendListItemDTO>>> GetFriendsList()
        {
            var result = await _mediator.Send(new GetFriendsListQuery());

            return result.ToActionResult();
        }

        [Authorize(Policy = "User")]
        [HttpGet("requests")]
        public async Task<ActionResult<List<FriendRequestDTO>>> GetFriendRequestsList()
        {
            var result = await _mediator.Send(new GetFriendRequestsListQuery());

            return result.ToActionResult();
        }

        [Authorize(Policy = "User")]
        [HttpPatch("accept-request")]
        public async Task<ActionResult> AcceptFriendRequest([FromBody] FriendRequestActionDTO dto)
        {
            var result = await _mediator.Send(new AcceptFriendRequestCommand(dto.FriendId));

            return result.ToActionResult();
        }

        [Authorize(Policy = "User")]
        [HttpDelete("decline-request")]
        public async Task<ActionResult> DeclineFriendRequest([FromBody] FriendRequestActionDTO dto)
        {
            var result = await _mediator.Send(new DeclineFriendRequestCommand(dto.FriendId));

            return result.ToActionResult();
        }

        [Authorize(Policy = "User")]
        [HttpPost("add-friend-by-code")]
        public async Task<ActionResult<AddFriendResponseDTO>> AddFriendByCode(AddFriendByCodeDTO dto)
        {
            var result = await _mediator.Send(new AddFriendByCodeCommand(dto.Code));

            return result.ToActionResult();
        }

        [Authorize(Policy = "User")]
        [HttpPost("request-friend-by-user-id")]
        public async Task<ActionResult<AddFriendResponseDTO>> RequestFriendByUserId(RequestFriendByUserIdDTO dto)
        {
            var result = await _mediator.Send(new RequestFriendByUserIdCommand(dto.FriendId));

            return result.ToActionResult();
        }
    }
}
