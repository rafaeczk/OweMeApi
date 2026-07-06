using Domain.Common;

namespace Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount has to be positive");

        Amount = amount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }

    public static implicit operator decimal(Money money) => money.Amount;

    public static explicit operator Money(decimal amount) => new(amount);
}
