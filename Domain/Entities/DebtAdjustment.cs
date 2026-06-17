using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public class DebtAdjustment : BaseEntity
{
    public Money Money { get; set; } = null!;
    public string Note { get; set; } = string.Empty;

    public LedgerEvent LedgerEvent { get; set; } = null!;
}
