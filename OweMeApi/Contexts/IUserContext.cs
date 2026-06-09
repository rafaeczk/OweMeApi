namespace OweMeApi.Contexts;

public interface IUserContext
{
    Guid Id { get; }
    string Role { get; }
}
