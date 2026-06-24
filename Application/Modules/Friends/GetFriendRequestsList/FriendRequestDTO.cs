namespace Application.Modules.Friends.GetFriendRequestsList;

public record FriendRequestDTO(Guid Id, string Email, string FullName, DateTime RequestedAt);
