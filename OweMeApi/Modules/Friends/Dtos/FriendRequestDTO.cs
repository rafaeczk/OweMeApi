namespace OweMeApi.Modules.Friends.Dtos;

public record FriendRequestDTO(Guid Id, string Email, string FullName, DateTime RequestedAt);
