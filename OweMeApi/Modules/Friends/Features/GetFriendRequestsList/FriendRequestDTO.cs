namespace OweMeApi.Modules.Friends.Features.GetFriendRequestsList;

public record FriendRequestDTO(Guid Id, string Email, string FullName, DateTime RequestedAt);
