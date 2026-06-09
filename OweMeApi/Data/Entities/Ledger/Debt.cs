using OweMeApi.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OweMeApi.Data.Entities.Ledger;

public class Debt
{
    [Key]
    public Guid Id { get; set; }

    public ICollection<LedgerEvent> LedgerEvents { get; private set; } = [];

    [Required]
    public required string Title { get; set; }

    public string? Description { get; set; }

    public Guid CreditorId { get; set; }
    public User Creditor { get; set; } = null!;

    public Guid DebtorId { get; set; }
    public User Debtor { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
