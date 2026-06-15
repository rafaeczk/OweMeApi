namespace OweMeApi.Modules.Users.Domain.Enums;

public static class UserRole
{
    public const string Admin = "Admin";
    public const string Moderator = "Moderator";
    public const string User = "User";
    public const string Locked = "Locked";

    public static string[] ValueList => [Admin, Moderator, User, Locked];
    public static bool Verify(string v) => ValueList.Contains(v);
}
