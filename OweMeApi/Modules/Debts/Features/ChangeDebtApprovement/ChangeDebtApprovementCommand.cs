using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Debts.Features.ChangeDebtApprovement;

public record ChangeDebtApprovementCommand(Guid UserId, Guid DebtId, bool Approve) : IRequest<HandlerResult>;
