namespace Domain.Enums;

public static class LedgerEventType
{
    public const string Adjustment = nameof(Adjustment);
    public const string Payment = nameof(Payment);
    public const string PaymentStatusChange = nameof(PaymentStatusChange);
    public const string CreditorDebtApprovement = nameof(CreditorDebtApprovement);
    public const string CreditorDebtDisapprovement = nameof(CreditorDebtDisapprovement);
    public const string DebtorDebtApprovement = nameof(DebtorDebtApprovement);
    public const string DebtorDebtDisapprovement = nameof(DebtorDebtDisapprovement);
    public const string DebtSettlement = nameof(DebtSettlement);

    public static string[] ValueList =>
    [
        Adjustment,
        Payment, PaymentStatusChange,
        CreditorDebtApprovement, CreditorDebtDisapprovement, DebtorDebtApprovement, DebtorDebtDisapprovement,
        DebtSettlement
    ];
    public static bool Verify(string v) => ValueList.Contains(v);
}
