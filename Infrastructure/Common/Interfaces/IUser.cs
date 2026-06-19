namespace Infrastructure.Common.Interfaces;

public interface IUser
{
    public Guid Id { get; }
    public string Role { get; }
}
