namespace Domain.Enums;

public static class DebtPaymentStatus
{
    public const string Success = nameof(Success);
    public const string Failure = nameof(Failure);
    public const string Pending = nameof(Pending);

    public static string[] ValueList => [Success, Failure, Pending];
    public static bool Verify(string v) => ValueList.Contains(v);
}
