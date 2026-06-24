using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class LedgerEvent : BaseAuditableEntity
{
    public Guid DebtId { get; private set; }
    public Debt Debt { get; private set; } = null!;
    public string EventType { get; private set; } = null!;
    public string InternalReference { get; private set; } = null!;

    // EVENT DETAILS

    public Guid? AdjustmentId { get; private set; }
    public DebtAdjustment? Adjustment { get; private set; }

    public Guid? PaymentId { get; private set; }
    public DebtPayment? Payment { get; private set; }

    public Guid? PaymentStatusChangeId { get; private set; }
    public DebtPaymentStatusChange? PaymentStatusChange { get; private set; }

    // METHODS

    private LedgerEvent() { }

    internal static LedgerEvent Create(Guid debtId, string eventType)
    {
        if (!LedgerEventTypes.Verify(eventType))
            throw new InvalidLedgerEventTypeException(eventType);

        return new()
        {
            DebtId = debtId,
            EventType = eventType,
            InternalReference = GenReferenceNumber()
        };
    }

    internal static LedgerEvent CreateAdjustment(Guid debtId, DebtAdjustment adjustment)
    {
        return new()
        {
            DebtId = debtId,
            AdjustmentId = adjustment.Id,
            Adjustment = adjustment,
            EventType = LedgerEventTypes.Adjustment,
            InternalReference = GenReferenceNumber()
        };
    }

    public static LedgerEvent CreatePayment(Guid debtId, Guid paymentId)
    {
        return new()
        {
            DebtId = debtId,
            PaymentId = paymentId,
            EventType = LedgerEventTypes.Adjustment,
            InternalReference = GenReferenceNumber()
        };
    }

    internal static LedgerEvent CreatePaymentStatusChange(Guid debtId, DebtPaymentStatusChange statusChange)
    {
        return new()
        {
            DebtId = debtId,
            PaymentStatusChangeId = statusChange.Id,
            PaymentStatusChange = statusChange,
            EventType = LedgerEventTypes.Adjustment,
            InternalReference = GenReferenceNumber()
        };
    }

    private static string GenReferenceNumber() => $"REF-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
}
