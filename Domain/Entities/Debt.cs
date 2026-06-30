using Domain.Common;
using Domain.Enums;
using Domain.Results;
using Domain.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities;

public class Debt : BaseAuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid CreditorId { get; private set; }
    public Guid DebtorId { get; private set; }

    public ICollection<LedgerEvent> LedgerEvents { get; private set; } = [];
    public User Creditor { get; private set; } = null!;
    public User Debtor { get; private set; } = null!;

    // METHODS

    private Debt() { }

    public void UpdateProfile(string title, string? description)
    {
        Title = title;
        Description = description;
    }

    public static Debt Create(string title, string? description, Guid creditorId, Guid debtorId)
    {
        return new()
        {
            Title = title,
            Description = description,
            CreditorId = creditorId,
            DebtorId = debtorId
        };
    }

    public LedgerEvent CreateApprovement(string eventType)
    {
        if (GetIsSettled())
            throw new DebtIsSettledException();

        if (!LedgerEventTypes.VerifyApprovement(eventType))
            throw new InvalidLedgerEventApprovementTypeException(eventType);

        var approvementEvent = LedgerEvent.Create(Id, eventType);

        LedgerEvents.Add(approvementEvent);

        return approvementEvent;
    }

    public LedgerEvent CreateSettlement()
    {
        if (GetIsSettled())
            throw new DebtIsSettledException();

        if(!GetCreditorApproves() || !GetDebtorApproves())
            throw new DebtIsNotFullyApprovedException();

        var settlementEvent = LedgerEvent.Create(Id, LedgerEventTypes.DebtSettlement);

        LedgerEvents.Add(settlementEvent);

        return settlementEvent;
    }

    public LedgerEvent CreateAdjustment(Money money, string note)
    {
        if (GetIsSettled())
            throw new DebtIsSettledException();

        var adjustment = DebtAdjustment.Create(money, note);

        var adjustmentEvent = LedgerEvent.CreateAdjustment(Id, adjustment);

        LedgerEvents.Add(adjustmentEvent);

        return adjustmentEvent;
    }

    public LedgerEvent CreatePayment(Money money, Guid payerId, Guid receiverId, string method, string? note)
    {
        if (GetIsSettled())
            throw new DebtIsSettledException();

        var payment = DebtPayment.Create(
            money,
            payerId,
            receiverId,
            method,
            note);

        var paymentEvent = LedgerEvent.CreatePayment(Id, payment);

        LedgerEvents.Add(paymentEvent);

        return paymentEvent;
    }

    // GETTERS

    public decimal GetTotalAmount()
    {
        return LedgerEvents
            .Where(e => e.EventType == LedgerEventTypes.Adjustment)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => e.Adjustment!.Money.Amount)
            .FirstOrDefault();
    }

    public decimal GetTotalPayments()
    {
        return LedgerEvents
            .Where(e => e.EventType == LedgerEventTypes.Payment)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => e.Payment)
            .Where(p => p != null)
            .Where(p => p!.StatusChangeEvents
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => e.PaymentStatusChange!.Status)
                .FirstOrDefault() == DebtPaymentStatus.Success)
            .Sum(p =>
                (p!.PayerId == p.LedgerEvent.Debt.CreditorId && p!.ReceiverId == p.LedgerEvent.Debt.DebtorId)
                    ? -p!.Money.Amount
                    : (p!.PayerId == p.LedgerEvent.Debt.DebtorId && p!.ReceiverId == p.LedgerEvent.Debt.CreditorId)
                        ? p!.Money.Amount
                        : 0m);
    }

    public static DebtSummaryResult CalcDebtSummary(decimal totalAmount, decimal totalPayments, Guid creditorId, Guid debtorId)
    {
        var diff = totalAmount - totalPayments;

        if (diff > 0)
            return new DebtSummaryResult(debtorId, creditorId, new Money(Math.Abs(diff)));
        else if (diff < 0)
            return new DebtSummaryResult(creditorId, debtorId, new Money(Math.Abs(diff)));
        else
            return new DebtSummaryResult(Guid.Empty, Guid.Empty, new Money(0m));
    }

    public bool GetCreditorApproves()
    {
        return LedgerEvents
            .Where(e => (e.EventType == LedgerEventTypes.CreditorDebtApprovement || e.EventType == LedgerEventTypes.CreditorDebtDisapprovement))
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => e.EventType == LedgerEventTypes.CreditorDebtApprovement)
            .FirstOrDefault();
    }

    public bool GetDebtorApproves()
    {
        return LedgerEvents
            .Where(e => (e.EventType == LedgerEventTypes.DebtorDebtApprovement || e.EventType == LedgerEventTypes.DebtorDebtDisapprovement))
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => e.EventType == LedgerEventTypes.DebtorDebtApprovement)
            .FirstOrDefault();
    }

    public bool GetIsSettled()
    {
        return LedgerEvents.Any(e => e.EventType == LedgerEventTypes.DebtSettlement);
    }

    public bool GetHasPendingPayments()
    {
        return LedgerEvents
            .Where(e => e.EventType == LedgerEventTypes.Payment)
            .Any(e => e.Payment!.StatusChangeEvents
                .OrderByDescending(e => e.CreatedAt)
                .First().PaymentStatusChange!.Status == DebtPaymentStatus.Pending);
    }
}
