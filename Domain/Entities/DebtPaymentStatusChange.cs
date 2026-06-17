using Domain.Common;

namespace Domain.Entities;

public class DebtPaymentStatusChange : BaseEntity
{
    public LedgerEvent LedgerEvent { get; set; } = null!;
    public Guid PaymentId { get; set; }
    public required string Status { get; set; }
    public string? Note { get; set; }

    public DebtPayment Payment { get; set; } = null!;
}
