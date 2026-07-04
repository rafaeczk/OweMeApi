using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class DebtPayment : BaseEntity
{
    public Guid LedgerEventId { get; private set; }
    public Money Money { get; private set; } = null!;
    public Guid PayerId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public string Method { get; private set; } = null!;
    public string? Note { get; private set; }

    public LedgerEvent LedgerEvent { get; private set; } = null!;
    public User Payer { get; private set; } = null!;
    public User Receiver { get; private set; } = null!;

    public ICollection<DebtPaymentStatusChange> StatusChanges { get; private set; } = [];

    private DebtPayment() { }

    internal static DebtPayment Create(Money money, Guid payerId, Guid receiverId, string method, string? note)
    {
        if (!DebtPaymentMethod.Verify(method))
            throw new InvalidPaymentMethodException(method);

        return new()
        {
            //Id = Guid.NewGuid(),
            Money = money,
            PayerId = payerId,
            ReceiverId = receiverId,
            Method = method,
            Note = note
        };
    }

    internal void SetLedgerEvent(LedgerEvent ledgerEvent)
    {
        LedgerEvent = ledgerEvent;
        LedgerEventId = ledgerEvent.Id;
    }

    public DebtPaymentStatusChange CreateStatusChange(string status, string? note)
    {
        var statusChange = DebtPaymentStatusChange.Create(this, status, note);

        var statusChangeEvent = LedgerEvent.CreatePaymentStatusChange(LedgerEvent.Debt, statusChange);
        statusChange.LedgerEvent = statusChangeEvent;

        StatusChanges.Add(statusChange);
        LedgerEvent.Debt.LedgerEvents.Add(statusChangeEvent);

        return statusChange;
    }
}
