namespace OweMeApi.Contexts.IUserContext;

public interface IUserContext
{
    Guid Id { get; }
    string Role { get; }
}
