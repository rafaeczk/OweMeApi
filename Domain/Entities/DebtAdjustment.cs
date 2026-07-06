using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public class DebtAdjustment : BaseEntity
{
    public Guid LedgerEventId { get; private set; }
    public Money Money { get; private set; } = null!;
    public string Note { get; private set; } = string.Empty;

    public LedgerEvent LedgerEvent { get; private set; } = null!;

    private DebtAdjustment() { }

    internal static DebtAdjustment Create(Money money, string note)
        => new()
        {
            Money = money,
            Note = note
        };

    internal void SetLedgerEvent(LedgerEvent ledgerEvent)
    {
        LedgerEvent = ledgerEvent;
        LedgerEventId = ledgerEvent.Id;
    }
}
