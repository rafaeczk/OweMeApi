using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public class DebtAdjustment : BaseEntity
{
    public Money Money { get; private set; } = null!;
    public string Note { get; private set; } = string.Empty;

    public LedgerEvent LedgerEvent { get; internal set; } = null!;

    private DebtAdjustment() { }

    internal static DebtAdjustment Create(Money money, string note)
        => new()
        {
            Money = money,
            Note = note
        };
}
