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

    internal static LedgerEvent Create(Debt debt, string eventType)
    {
        if (!LedgerEventTypes.Verify(eventType))
            throw new InvalidLedgerEventTypeException(eventType);

        return new()
        {
            Id = Guid.NewGuid(),
            DebtId = debt.Id,
            Debt = debt,
            EventType = eventType,
            InternalReference = GenReferenceNumber()
        };
    }

    internal static LedgerEvent CreateAdjustment(Debt debt, DebtAdjustment adjustment)
    {
        var adjustmentEvent = new LedgerEvent()
        {
            Id = Guid.NewGuid(),
            DebtId = debt.Id,
            Debt = debt,
            AdjustmentId = adjustment.Id,
            Adjustment = adjustment,
            EventType = LedgerEventTypes.Adjustment,
            InternalReference = GenReferenceNumber()
        };

        adjustment.LedgerEvent = adjustmentEvent;

        return adjustmentEvent;
    }

    internal static LedgerEvent CreatePayment(Debt debt, DebtPayment payment)
    {
        var paymentEvent = new LedgerEvent()
        {
            Id = Guid.NewGuid(),
            DebtId = debt.Id,
            Debt = debt,
            PaymentId = payment.Id,
            Payment = payment,
            EventType = LedgerEventTypes.Payment,
            InternalReference = GenReferenceNumber()
        };

        payment.LedgerEvent = paymentEvent;

        return paymentEvent;
    }

    internal static LedgerEvent CreatePaymentStatusChange(Debt debt, DebtPaymentStatusChange statusChange)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            DebtId = debt.Id,
            Debt = debt,
            PaymentStatusChangeId = statusChange.Id,
            PaymentStatusChange = statusChange,
            EventType = LedgerEventTypes.PaymentStatusChange,
            InternalReference = GenReferenceNumber()
        };
    }

    private static string GenReferenceNumber() => $"REF-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
}
