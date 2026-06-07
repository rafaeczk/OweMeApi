namespace OweMeApi.Modules.Friends.Dtos;

public record FriendListItemDTO(Guid Id, string Email, string FullName, DateTime Since);
