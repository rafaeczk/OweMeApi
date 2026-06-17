using Domain.Common;

namespace Domain.Enums;

public static class DebtPaymentMethod
{
    public const string Cash = nameof(Cash);
    public const string Transfer = nameof(Transfer);

    public static string[] ValueList => [Cash, Transfer];
    public static bool Verify(string v) => ValueList.Contains(v);
}
