namespace Domain.Enums;

public static class UserRole
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);

    public static string[] ValueList => [Admin, User];
    public static bool Verify(string v) => ValueList.Contains(v);
}
