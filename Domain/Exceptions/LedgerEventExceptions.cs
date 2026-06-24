using Domain.Common;
using Domain.Enums;

namespace Domain.Exceptions;

public class InvalidLedgerEventTypeException(string eventType)
    : DomainException($"Invalid ledger event type: {eventType}. Available event types: {string.Join(", ", LedgerEventTypes.ValueList)}");

public class InvalidLedgerEventApprovementTypeException(string eventType)
    : DomainException(
        $"Invalid ledger event type: {eventType}. " +
        $"Available event types: {string.Join(", ", LedgerEventTypes.ApprovementsValueList)}");
