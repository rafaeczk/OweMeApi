namespace Domain.Enums;

public static class UserRole
{
    public const string Admin = nameof(Admin);
    public const string Moderator = nameof(Moderator);
    public const string User = nameof(User);
    public const string Locked = nameof(Locked);

    public static string[] ValueList => [Admin, Moderator, User, Locked];
    public static bool Verify(string v) => ValueList.Contains(v);
}
