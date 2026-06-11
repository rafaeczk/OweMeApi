namespace OweMeApi.Modules.Debts.Domain.Enums;

public static class LedgerEventType
{
    public const string Adjustment = "Adjustment";
    public const string Payment = "Payment";
    public const string PaymentStatusChange = "PaymentStatusChange";
    public const string CreditorDebtApprovement = "CreditorDebtApprovement";
    public const string CreditorDebtDisapprovement = "CreditorDebtDisapprovement";
    public const string DebtorDebtApprovement = "DebtorDebtApprovement";
    public const string DebtorDebtDisapprovement = "DebtorDebtDisapprovement";
    public const string DebtSettlement = "DebtSettlement";

    public static string[] ValueList =>
    [
        Adjustment,
        Payment, PaymentStatusChange,
        CreditorDebtApprovement, CreditorDebtDisapprovement, DebtorDebtApprovement, DebtorDebtDisapprovement,
        DebtSettlement
    ];
    public static bool Verify(string v) => ValueList.Contains(v);
}
