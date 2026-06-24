namespace Application.Common.Interfaces;

public interface IUserContext
{
    Guid Id { get; }
    string Role { get; }
}
