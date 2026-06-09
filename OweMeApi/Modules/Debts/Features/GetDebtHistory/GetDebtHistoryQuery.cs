using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Debts.Dtos;

namespace OweMeApi.Modules.Debts.Features.GetDebtHistory;

public record GetDebtHistoryQuery(Guid UserId, Guid DebtId) : IRequest<HandlerResult<List<DebtHistoryListItemDTO>>>;
