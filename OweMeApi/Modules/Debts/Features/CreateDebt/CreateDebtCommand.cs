using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Debts.Features.CreateDebt;

public record CreateDebtCommand(Guid DebtorId, string Title, string? Description, decimal Amount) : IRequest<HandlerResult<Guid>>;
