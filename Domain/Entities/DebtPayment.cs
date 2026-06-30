using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class DebtPayment : BaseEntity
{
    public Money Money { get; private set; } = null!;
    public Guid PayerId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public string Method { get; private set; } = null!;
    public string? Note { get; private set; }

    public ICollection<LedgerEvent> StatusChangeEvents { get; private set; } = [];
    public LedgerEvent LedgerEvent { get; internal set; } = null!;
    public User Payer { get; private set; } = null!;
    public User Receiver { get; private set; } = null!;

    private DebtPayment() { }

    internal static DebtPayment Create(Money money, Guid payerId, Guid receiverId, string method, string? note)
    {
        if (!DebtPaymentMethod.Verify(method))
            throw new InvalidPaymentMethodException(method);

        return new()
        {
            Money = money,
            PayerId = payerId,
            ReceiverId = receiverId,
            Method = method,
            Note = note
        };
    }

    public void CreateStatusChange(string status, string? note)
    {
        var statusChange = DebtPaymentStatusChange.Create(Id, status, note);

        var statusChangeEvent = LedgerEvent.CreatePaymentStatusChange(LedgerEvent.DebtId, statusChange);

        StatusChangeEvents.Add(statusChangeEvent);
    }
}
