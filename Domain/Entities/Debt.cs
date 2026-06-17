using Domain.Common;

namespace Domain.Entities;

public class Debt : BaseAuditableEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid CreditorId { get; set; }
    public Guid DebtorId { get; set; }

    public ICollection<LedgerEvent> LedgerEvents { get; private set; } = [];
    public User Creditor { get; set; } = null!;
    public User Debtor { get; set; } = null!;
}
