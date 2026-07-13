using Application.Common;
using Application.Common.Pagination;
using Application.Modules.Debts.ChangeDebtAmount;
using Application.Modules.Debts.ChangeDebtApprovement;
using Application.Modules.Debts.CreateDebt;
using Application.Modules.Debts.CreatePayment;
using Application.Modules.Debts.EditDebtInformation;
using Application.Modules.Debts.GetDebt;
using Application.Modules.Debts.GetDebtHistory;
using Application.Modules.Debts.GetDebts;
using Application.Modules.Debts.VerifyPayment;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers;

[Route("api/debts")]
[ApiController]
[Authorize(Policy = AuthPolicies.AdminOrUser)]
public class DebtsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<PagedResult<DebtListItemDTO>>> GetDebts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery, AllowedValues("creditor", "debtor", "any")] string role = "any",
        [FromQuery, AllowedValues("settled", "unsettled", "any")] string state = "any")
    {
        QEUserRoleInDebt userRoleInDebt = role switch
        {
            "creditor" => QEUserRoleInDebt.Creditor,
            "debtor" => QEUserRoleInDebt.Debtor,
            _ => QEUserRoleInDebt.Any
        };

        QEDebtState debtState = state switch
        {
            "settled" => QEDebtState.Settled,
            "unsettled" => QEDebtState.Unsettled,
            _ => QEDebtState.Any,
        };

        var result = await _mediator.Send(new GetDebtsQuery(userRoleInDebt, debtState, new(pageNumber, pageSize)));

        return result.ToActionResult();
    }

    [HttpGet("{debtId}")]
    public async Task<ActionResult<DebtDTO>> GetDebt(Guid debtId)
    {
        var result = await _mediator.Send(new GetDebtQuery(debtId));

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateDebt([FromBody] CreateDebtDTO dto)
    {
        var result = await _mediator.Send(new CreateDebtCommand(dto.DebtorId, dto.Title, dto.Description, dto.Amount));

        return result.ToActionResult();
    }

    [HttpPut("{debtId}")]
    public async Task<ActionResult<Guid>> CreateDebt(Guid debtId, [FromBody] EditDebtInformationDTO dto)
    {
        var result = await _mediator.Send(new EditDebtInformationCommand(debtId, dto.Title, dto.Description));

        return result.ToActionResult();
    }

    [HttpPost("create-payment")]
    public async Task<ActionResult<Guid>> CreatePayment([FromBody] CreatePaymentDTO dto)
    {
        var result = await _mediator.Send(new CreatePaymentCommand(dto.DebtId, dto.Amount, dto.Note, dto.PaymentMethod));

        return result.ToActionResult();
    }

    [HttpPatch("verify-payment")]
    public async Task<ActionResult> VerifyPayment([FromBody] VerifyPaymentDTO dto)
    {
        var result = await _mediator.Send(new VerifyPaymentCommand(dto.PaymentId, dto.Status, dto.Note));

        return result.ToActionResult();
    }

    [HttpGet("{debtId}/history")]
    public async Task<ActionResult<List<DebtHistoryListItemDTO>>> GetDebtHistory(Guid debtId)
    {
        var result = await _mediator.Send(new GetDebtHistoryQuery(debtId));

        return result.ToActionResult();
    }

    [HttpPatch("{debtId}/change-amount")]
    public async Task<ActionResult> ChangeDebtAmount(Guid debtId, [FromBody] ChangeDebtAmountDTO dto)
    {
        var result = await _mediator.Send(new ChangeDebtAmountCommand(debtId, dto.Amount, dto.Note));

        return result.ToActionResult();
    }

    [HttpPatch("{debtId}/approvement")]
    public async Task<ActionResult> ChangeDebtApprovement(Guid debtId, [FromBody] ChangeDebtApprovementDTO dto)
    {
        var result = await _mediator.Send(new ChangeDebtApprovementCommand(debtId, dto.Approve));

        return result.ToActionResult();
    }
}
