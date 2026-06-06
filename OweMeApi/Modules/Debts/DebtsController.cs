using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Data.Entities;
using OweMeApi.Helpers;
using OweMeApi.Modules.Debts.Dtos;
using System.ComponentModel.DataAnnotations;

namespace OweMeApi.Modules.Debts;

[Route("api/debts")]
[ApiController]
public class DebtsController(AppDbContext context, DebtsService debtsService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly DebtsService _debtsService = debtsService;

    [HttpGet]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<List<DebtInformationDTO>>> GetDebts([FromQuery, AllowedValues("creditor", "debtor", "any")] string role = "any")
    {
        var (userIdOk, userId) = AuthHelpers.GetUserId(User);

        if (!userIdOk)
            return BadRequest("Invalid token");

        IQueryable<Debt> debtsQuery = _context.Debts;

        debtsQuery = role switch
        {
            "creditor" => debtsQuery.Where(d => d.CreditorId == userId),
            "debtor" => debtsQuery.Where(d => d.DebtorId == userId),
            _ => debtsQuery.Where(d => d.CreditorId == userId || d.DebtorId == userId)
        };

        var debts = await debtsQuery.ToListAsync();
        var result = new List<DebtInformationListItemDTO>();

        foreach (var d in debts)
        {
            var (totalAmount, totalPayments) = await _debtsService.CalcDebtTotals(d);

            result.Add(
                new DebtInformationListItemDTO(
                    d.Id,
                    d.Title,
                    d.Description,
                    d.CreditorId,
                    d.DebtorId,
                    totalAmount,
                    totalPayments,
                    d.CreditorApproves,
                    d.DebtorApproves,
                    d.IsSettled
                )
            );
        }

        return Ok(result);
    }

    [HttpGet("{debtId}")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<DebtInformationDTO>> GetDebt(Guid debtId)
    {
        var (userIdOk, userId) = AuthHelpers.GetUserId(User);

        if (!userIdOk)
            return BadRequest("Invalid token");

        var debt = await _context.Debts.FirstOrDefaultAsync(d => d.Id == debtId && (d.CreditorId == userId || d.DebtorId == userId));

        if (debt == null)
            return NotFound();

        var (totalAmount, totalPayments, summary) = await _debtsService.CalcDebtSummary(debt);

        DebtInformationDTO result = new(
            debt.Id,
            debt.Title,
            debt.Description,
            debt.CreditorId,
            debt.DebtorId,
            totalAmount,
            totalPayments,
            summary,
            debt.CreditorApproves,
            debt.DebtorApproves,
            debt.IsSettled
        );

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<string>> CreateDebt([FromBody] CreateDebtDTO dto)
    {
        var (userIdOk, userId) = AuthHelpers.GetUserId(User);

        if (!userIdOk)
            return BadRequest("Invalid token");

        Debt debt = new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Amount = Math.Abs(dto.Amount),
            DebtorId = dto.DebtorId,
            CreditorId = userId
        };

        _context.Debts.Add(debt);
        await _context.SaveChangesAsync();

        return Ok(debt.Id);
    }

    [HttpPut("{debtId}")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult> EditDebtData([FromBody] EditDebtDataDTO dto, Guid debtId)
    {
        var debt = await _context.Debts.FirstOrDefaultAsync(d => d.Id == debtId);

        if (debt == null)
            return NotFound();

        debt.Title = dto.Title;
        debt.Description = dto.Description;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{debtId}/change-approvement")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult> ChangeDebtApprovement(Guid debtId, [FromBody] ChangeDebtApprovementDTO dto)
    {
        var (userIdOk, userId) = AuthHelpers.GetUserId(User);

        if (!userIdOk)
            return BadRequest("Invalid token");

        var debt = await _context.Debts.FirstOrDefaultAsync(d => d.Id == debtId);

        if (debt == null)
            return NotFound();

        try
        {
            if (debt.CreditorId == userId)
                debt.ToggleCreditorApproval(dto.Approved);
            else if (debt.DebtorId == userId)
                debt.ToggleDebtorApproval(dto.Approved);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
