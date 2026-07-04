using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class DebtPaymentStatusChange : BaseEntity
{
    public Guid LedgerEventId { get; private set; }
    public Guid PaymentId { get; private set; }
    public string Status { get; private set; } = null!;
    public string? Note { get; private set; }

    public LedgerEvent LedgerEvent { get; internal set; } = null!;

    public DebtPayment Payment { get; private set; } = null!;

    private DebtPaymentStatusChange() { }

    internal static DebtPaymentStatusChange Create(DebtPayment payment, string status, string? note)
    {
        if (!DebtPaymentStatus.Verify(status))
            throw new InvalidPaymentStatusException(status);

        return new()
        {
            Id = Guid.NewGuid(),
            PaymentId = payment.Id,
            Payment = payment,
            Status = status,
            Note = note
        };
    }

    internal void SetLedgerEvent(LedgerEvent ledgerEvent)
    {
        LedgerEvent = ledgerEvent;
        LedgerEventId = ledgerEvent.Id;
    }
}
