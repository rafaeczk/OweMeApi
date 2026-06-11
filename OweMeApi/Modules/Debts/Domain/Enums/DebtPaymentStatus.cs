namespace OweMeApi.Modules.Debts.Domain.Enums;

public static class DebtPaymentStatus
{
    public const string Success = "Success";
    public const string Failure = "Failure";
    public const string Pending = "Pending";

    public static string[] ValueList => [Success, Failure, Pending];
    public static bool Verify(string v) => ValueList.Contains(v);
}
