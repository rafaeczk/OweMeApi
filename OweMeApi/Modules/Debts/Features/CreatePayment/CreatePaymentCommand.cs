using MediatR;
using OweMeApi.Common;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.CreatePayment;

public record CreatePaymentCommand(Guid DebtId, decimal Amount, string? Note, PaymentMethod PaymentMethod) : IRequest<HandlerResult<Guid>>;
