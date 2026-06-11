namespace OweMeApi.Modules.Debts.Domain.Enums;

public static class DebtPaymentMethod
{
    public const string Cash = "Cash";
    public const string Transfer = "Transfer";

    public static string[] ValueList => [Cash, Transfer];
    public static bool Verify(string v) => ValueList.Contains(v);
}
