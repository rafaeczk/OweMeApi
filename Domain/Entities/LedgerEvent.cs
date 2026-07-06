using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class LedgerEvent : BaseAuditableEntity
{
    public Guid DebtId { get; private set; }
    public string EventType { get; private set; } = null!;
    public string InternalReference { get; private set; } = null!;

    public Debt Debt { get; private set; } = null!;

    // EVENT DETAILS
    public DebtAdjustment? Adjustment { get; private set; }
    public DebtPayment? Payment { get; private set; }
    public DebtPaymentStatusChange? PaymentStatusChange { get; private set; }

    // METHODS

    private LedgerEvent() { }

    internal static LedgerEvent Create(Debt debt, string eventType)
    {
        if (!LedgerEventTypes.Verify(eventType))
            throw new InvalidLedgerEventTypeException(eventType);

        return new()
        {
            //Id = Guid.NewGuid(),
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
            Adjustment = adjustment,
            EventType = LedgerEventTypes.Adjustment,
            InternalReference = GenReferenceNumber()
        };

        adjustment.SetLedgerEvent(adjustmentEvent);

        return adjustmentEvent;
    }

    internal static LedgerEvent CreatePayment(Debt debt, DebtPayment payment)
    {
        var paymentEvent = new LedgerEvent()
        {
            Id = Guid.NewGuid(),
            DebtId = debt.Id,
            Debt = debt,
            Payment = payment,
            EventType = LedgerEventTypes.Payment,
            InternalReference = GenReferenceNumber()
        };

        payment.SetLedgerEvent(paymentEvent);

        return paymentEvent;
    }

    internal static LedgerEvent CreatePaymentStatusChange(Debt debt, DebtPaymentStatusChange statusChange)
    {
        var statusChangeEvent = new LedgerEvent()
        {
            Id = Guid.NewGuid(),
            DebtId = debt.Id,
            Debt = debt,
            PaymentStatusChange = statusChange,
            EventType = LedgerEventTypes.PaymentStatusChange,
            InternalReference = GenReferenceNumber()
        };

        statusChange.SetLedgerEvent(statusChangeEvent);

        return statusChangeEvent;
    }

    private static string GenReferenceNumber() => $"REF-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
}
