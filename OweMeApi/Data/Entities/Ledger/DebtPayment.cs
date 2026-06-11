using System.ComponentModel.DataAnnotations;

namespace OweMeApi.Data.Entities.Ledger;

public class DebtPayment
{
    [Key]
    public Guid Id { get; set; }

    public LedgerEvent LedgerEvent { get; set; } = null!;

    public ICollection<DebtPaymentStatusChange> StatusChanges { get; set; } = [];

    public decimal Amount { get; set; }

    public Guid PayerId { get; set; }
    public User Payer { get; set; } = null!;

    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;

    public required string Method { get; set; }

    public string? Note { get; set; }
}
