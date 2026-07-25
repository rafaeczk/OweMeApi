using Domain.Common;
using Domain.Enums;
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

    public LedgerEvent CreateApprovement(Guid actorId, string eventType)
    {
        EnsureCanChangeApprovement(actorId);

        if (!LedgerEventTypes.VerifyApprovement(eventType))
            throw new InvalidLedgerEventApprovementTypeException(eventType);

        var approvementEvent = LedgerEvent.Create(this, eventType);

        LedgerEvents.Add(approvementEvent);

        return approvementEvent;
    }

    public LedgerEvent CreateSettlement()
    {
        EnsureCanBeSettled();

        var settlementEvent = LedgerEvent.Create(this, LedgerEventTypes.DebtSettlement);

        LedgerEvents.Add(settlementEvent);

        return settlementEvent;
    }

    public DebtAdjustment CreateAdjustment(Guid actorId, Money money, string note)
    {
        EnsureCanChangeAmount(actorId);

        var adjustment = DebtAdjustment.Create(money, note);

        var adjustmentEvent = LedgerEvent.CreateAdjustment(this, adjustment);

        LedgerEvents.Add(adjustmentEvent);

        return adjustment;
    }

    public DebtPayment CreatePayment(Guid actorId, Money money, Guid payerId, Guid receiverId, string method, string? note)
    {
        EnsureCanCreatePayment(actorId);

        var payment = DebtPayment.Create(
            money,
            payerId,
            receiverId,
            method,
            note);

        var paymentEvent = LedgerEvent.CreatePayment(this, payment);

        LedgerEvents.Add(paymentEvent);

        return payment;
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
            .Where(p => p!.StatusChanges
                .OrderByDescending(p => p.LedgerEvent.CreatedAt)
                .Select(e => e.Status)
                .FirstOrDefault() == DebtPaymentStatus.Success)
            .Sum(p =>
                (p!.PayerId == p.LedgerEvent.Debt.CreditorId && p!.ReceiverId == p.LedgerEvent.Debt.DebtorId)
                    ? -p!.Money.Amount
                    : (p!.PayerId == p.LedgerEvent.Debt.DebtorId && p!.ReceiverId == p.LedgerEvent.Debt.CreditorId)
                        ? p!.Money.Amount
                        : 0m);
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

    public bool GetParticipantApproves(Guid userId)
    {
        if (userId == CreditorId)
            return GetCreditorApproves();
        else if (userId == DebtorId)
            return GetDebtorApproves();
        else
            return false;
    }

    public bool GetIsSettled()
    {
        return LedgerEvents.Any(e => e.EventType == LedgerEventTypes.DebtSettlement);
    }

    public bool GetHasPendingPayments()
    {
        return LedgerEvents
            .Where(e => e.EventType == LedgerEventTypes.Payment)
            .Any(e => e.Payment!.StatusChanges
                .OrderByDescending(e => e.LedgerEvent.CreatedAt)
                .First().Status == DebtPaymentStatus.Pending);
    }

    // DOMAIN GUARDS

    public void EnsureCanChangeApprovement(Guid userId)
    {
        EnsureIsParticipant(userId);
        EnsureNotSettled();
        EnsureNoPendingPayments();
    }

    public void EnsureCanBeSettled()
    {
        EnsureNotSettled();

        if (!GetCreditorApproves() || !GetDebtorApproves())
            throw new DebtIsNotFullyApprovedException();
    }

    public void EnsureCanCreatePayment(Guid userId)
    {
        EnsureIsParticipant(userId);
        EnsureNotSettled();
    }

    public void EnsureCanChangeAmount(Guid userId)
    {
        EnsureIsCreditor(userId);
        EnsureNotSettled();
    }

    public void EnsureCanChangeInformation(Guid userId)
    {
        EnsureIsCreditor(userId);
    }

    private void EnsureNotSettled()
    {
        if (GetIsSettled())
            throw new DebtIsSettledException();
    }

    private void EnsureNoPendingPayments()
    {
        if (GetHasPendingPayments())
            throw new DebtHasPendingPaymentsException();
    }

    private void EnsureIsParticipant(Guid userId)
    {
        if (userId != CreditorId && userId != DebtorId)
            throw new UnauthorizedDebtAccessException();
    }

    private void EnsureIsCreditor(Guid userId)
    {
        if(userId != CreditorId)
            throw new UnauthorizedDebtAccessException();
    }

    // AVAILABLE ACTIONS

    public bool GetUserCanChangeApprovement(Guid userId)
    {
        try
        {
            EnsureCanChangeApprovement(userId);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }

    public bool GetUserCanChangeInformation(Guid userId)
    {
        try
        {
            EnsureCanChangeInformation(userId);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }

    public bool GetUserCanChangeAmount(Guid userId)
    {
        try
        {
            EnsureCanChangeAmount(userId);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }

    public bool GetUserCanCreatePayment(Guid userId)
    {
        try
        {
            EnsureCanCreatePayment(userId);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }
}
