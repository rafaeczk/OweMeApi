using System.ComponentModel.DataAnnotations;

namespace OweMeApi.Data.Entities.Ledger;

public class DebtPaymentStatusChange
{
    [Key]
    public Guid Id { get; set; }

    public LedgerEvent LedgerEvent { get; set; } = null!;

    public Guid PaymentId { get; set; }
    public DebtPayment Payment { get; set; } = null!;

    public required string Status { get; set; }

    public string? Note { get; set; }
}
