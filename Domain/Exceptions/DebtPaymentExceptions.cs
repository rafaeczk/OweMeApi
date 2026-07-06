using Domain.Common;
using Domain.Enums;

namespace Domain.Exceptions;

public class InvalidPaymentStatusException(string status) 
    : DomainException($"Invalid payment status: {status}. Available statuses: {string.Join(", ", DebtPaymentStatus.ValueList)}");

public class InvalidPaymentMethodException(string method)
    : DomainException($"Invalid payment method: {method}. Available methods: {string.Join(", ", DebtPaymentMethod.ValueList)}");

public class DebtIsSettledException() : DomainException("Debt is already settled");

public class DebtIsNotFullyApprovedException() : DomainException("Debt is not approved from both sides");
