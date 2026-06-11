using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OweMeApi.Modules.Debts.Features.CreateDebt;
using OweMeApi.Modules.Debts.Features.GetDebts;
using OweMeApi.Modules.Debts.Features.GetDebt;
using System.ComponentModel.DataAnnotations;
using OweMeApi.Modules.Debts.Features.CreatePayment;
using OweMeApi.Modules.Debts.Features.VerifyCashPayment;
using OweMeApi.Modules.Debts.Features.GetDebtHistory;
using OweMeApi.Modules.Debts.Features.ChangeDebtAmount;
using OweMeApi.Modules.Debts.Features.ChangeDebtApprovement;
using OweMeApi.Modules.Debts.Features.EditDebtInformation;

namespace OweMeApi.Modules.Debts;

[Route("api/debts")]
[ApiController]
public class DebtsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<List<DebtListItemDTO>>> GetDebts(
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

        var result = await _mediator.Send(new GetDebtsQuery(userRoleInDebt, debtState));

        return result.ToActionResult(HttpContext);
    }

    [HttpGet("{debtId}")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<DebtDTO>> GetDebt(Guid debtId)
    {
        var result = await _mediator.Send(new GetDebtQuery(debtId));

        return result.ToActionResult(HttpContext);
    }

    [HttpPost]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<Guid>> CreateDebt([FromBody] CreateDebtDTO dto)
    {
        var result = await _mediator.Send(new CreateDebtCommand(dto.DebtorId, dto.Title, dto.Description, dto.Amount));

        return result.ToActionResult(HttpContext);
    }

    [HttpPut("{debtId}")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<Guid>> CreateDebt(Guid debtId, [FromBody] EditDebtInformationDTO dto)
    {
        var result = await _mediator.Send(new EditDebtInformationCommand(debtId, dto.Title, dto.Description));

        return result.ToActionResult(HttpContext);
    }

    [HttpPost("create-payment")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<Guid>> CreatePayment([FromBody] CreatePaymentDTO dto)
    {
        var result = await _mediator.Send(new CreatePaymentCommand(dto.DebtId, dto.Amount, dto.Note, dto.PaymentMethod));

        return result.ToActionResult(HttpContext);
    }

    [HttpPatch("verify-cash-payment")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult> VerifyCashPayment([FromBody] VerifyCashPaymentDTO dto)
    {
        var result = await _mediator.Send(new VerifyCashPaymentCommand(dto.PaymentId, dto.Status, dto.Note));

        return result.ToActionResult(HttpContext);
    }

    [HttpGet("{debtId}/history")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<List<DebtHistoryListItemDTO>>> GetDebtHistory(Guid debtId)
    {
        var result = await _mediator.Send(new GetDebtHistoryQuery(debtId));

        return result.ToActionResult(HttpContext);
    }

    [HttpPatch("{debtId}/change-amount")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult> ChangeDebtAmount(Guid debtId, [FromBody] ChangeDebtAmountDTO dto)
    {
        var result = await _mediator.Send(new ChangeDebtAmountCommand(debtId, dto.Amount, dto.Note));

        return result.ToActionResult(HttpContext);
    }

    [HttpPatch("{debtId}/approvement")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult> ChangeDebtApprovement(Guid debtId, [FromBody] ChangeDebtApprovementDTO dto)
    {
        var result = await _mediator.Send(new ChangeDebtApprovementCommand(debtId, dto.Approve));

        return result.ToActionResult(HttpContext);
    }
}
