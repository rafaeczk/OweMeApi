namespace OweMeApi.Modules.Friends.Features.GetFriendsList;

public record FriendListItemDTO(Guid Id, string Email, string FullName, DateTime Since);
