using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class DebtPaymentStatusChange : BaseEntity
{
    public LedgerEvent LedgerEvent { get; private set; } = null!;
    public Guid PaymentId { get; private set; }
    public string Status { get; private set; } = null!;
    public string? Note { get; private set; }

    public DebtPayment Payment { get; private set; } = null!;

    private DebtPaymentStatusChange() { }

    internal static DebtPaymentStatusChange Create(Guid paymentId, string status, string? note)
    {
        if (!DebtPaymentStatus.Verify(status))
            throw new InvalidPaymentStatusException(status);

        return new()
        {
            PaymentId = paymentId,
            Status = status,
            Note = note
        };
    }
}
