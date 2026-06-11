using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Debts.Features.CreatePayment;

public record CreatePaymentCommand(Guid DebtId, decimal Amount, string? Note, string PaymentMethod) : IRequest<HandlerResult<Guid>>;
