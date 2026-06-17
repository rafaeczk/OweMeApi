using Domain.ValueObjects;

namespace Domain.Entities;

public class DebtPayment
{
    public Money Money { get; set; } = null!;
    public Guid PayerId { get; set; }
    public Guid ReceiverId { get; set; }
    public required string Method { get; set; }
    public string? Note { get; set; }

    public ICollection<DebtPaymentStatusChange> StatusChanges { get; set; } = [];
    public LedgerEvent LedgerEvent { get; set; } = null!;
    public User Payer { get; set; } = null!;
    public User Receiver { get; set; } = null!;
}
