using System.ComponentModel.DataAnnotations;

namespace OweMeApi.Data.Entities.Ledger;

public class DebtAdjustment
{
    [Key]
    public Guid Id { get; set; }

    public LedgerEvent LedgerEvent { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Note { get; set; } = string.Empty;
}
