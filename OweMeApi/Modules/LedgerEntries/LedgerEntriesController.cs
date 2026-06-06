using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Data.Entities;
using OweMeApi.Extensions;
using OweMeApi.Modules.LedgerEntries.Dtos;
using System.ComponentModel.DataAnnotations;

namespace OweMeApi.Modules.LedgerEntries;

[Route("api/ledger-entries")]
[ApiController]
public class LedgerEntriesController(AppDbContext context, LedgerEntriesService ledgerEntriesService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly LedgerEntriesService _ledgerEntriesService = ledgerEntriesService;

    [HttpGet("debt/{debtId}/history")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult<List<LedgerEntryListItemDTO>>> GetDebtHistory(
        Guid debtId,
        [FromQuery, AllowedValues("payment", "adjustment", "any")] string transactionType = "any")
    {
        var userId = User.GetUserId();

        var debt = await _context.Debts.FirstOrDefaultAsync(d => d.Id == debtId && (d.CreditorId == userId || d.DebtorId == userId));

        if (debt == null)
            return NotFound();

        var entriesQuery = _context.LedgerEntries
            .Where(e => e.DebtId == debtId);

        entriesQuery = transactionType switch
        {
            "payment" => entriesQuery.Where(e => e.TransactionType == TransactionType.Payment),
            "adjustment" => entriesQuery.Where(e => e.TransactionType == TransactionType.Adjustment),
            _ => entriesQuery
        };

        var entries = await entriesQuery.OrderByDescending(e => e.CreatedAt).ToListAsync();
        var result = new List<LedgerEntryListItemDTO>();

        foreach (var e in entries)
        {
            result.Add(
                new LedgerEntryListItemDTO(
                    e.Id,
                    e.InternalReference,
                    e.ExternalReference,
                    e.Amount,
                    await _ledgerEntriesService.CalcCurrentDebtAmount(debt, e.CreatedAt),
                    e.Note,
                    e.TransactionType,
                    e.PaymentMethod,
                    e.PaymentStatus,
                    e.CreatedByUserId,
                    e.CreatedAt
                )
            );
        }

        return Ok(result);
    }

    [HttpPost("create-payment")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<string>> CreatePayment([FromBody] CreatePaymentDTO dto)
    {
        var userId = User.GetUserId();

        var debt = await _context.Debts.FirstOrDefaultAsync(d => d.Id == dto.DebtId);

        if (debt == null)
            return NotFound();

        if (debt.IsSettled)
            return BadRequest("The debt is already settled");

        decimal amount = Math.Abs(dto.Amount);

        if (userId == debt.CreditorId)
            amount *= -1;
        if (userId == debt.DebtorId)
            amount *= 1;

        LedgerEntry entry = new()
        {
            InternalReference = LedgerEntry.GenReferenceNumber,
            DebtId = dto.DebtId,
            Amount = amount,
            Note = dto.Note,
            TransactionType = TransactionType.Payment,
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            CreatedByUserId = userId
        };

        _context.LedgerEntries.Add(entry);
        await _context.SaveChangesAsync();

        // TODO: kiedy dodam bramki zwrócimy url do płatności

        return Ok(entry.Id);
    }

    [HttpPost("{entryId}/verify-cash-payment")]
    [Authorize(Policy = "All")]
    public async Task<ActionResult> VerifyCashPayment(Guid entryId, [FromBody] VerifyCashPaymentDTO dto)
    {
        var userId = User.GetUserId();

        var entry = await _context.LedgerEntries
            .Include(e => e.Debt)
            .FirstOrDefaultAsync(e =>
                (e.CreatedByUserId == e.Debt.CreditorId || e.CreatedByUserId == e.Debt.DebtorId)
                && e.Id == entryId
                && e.TransactionType == TransactionType.Payment
                && e.PaymentMethod == PaymentMethod.Cash
            );

        if (entry == null)
            return NotFound();

        if (entry.CreatedByUserId == userId)
            return Unauthorized("You cannot accept your own cash payment");

        if (entry.PaymentStatus != PaymentStatus.Pending)
            return BadRequest("You can verify payment only once");

        entry.PaymentStatus = dto.Accept ? PaymentStatus.Confirmed : PaymentStatus.Rejected;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("debt/{debtId}/amount")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult> ChangeDebtAmount([FromBody] ChangeDebtAmountDTO dto, Guid debtId)
    {
        var userId = User.GetUserId();

        var debt = await _context.Debts.FirstOrDefaultAsync(d => d.Id == debtId);

        if (debt == null)
            return NotFound();

        if (debt.IsSettled)
            return BadRequest("The debt is already settled");

        if (debt.CreditorId != userId)
            return Unauthorized("The debtor cannot change debt amount");

        if (dto.Amount <= 0)
            return BadRequest("Amount cannot be neither negative nor zero");

        var adjustmentsSum = await _ledgerEntriesService.CalcAdjustmentsSum(debtId);

        decimal adjustmentAmount = dto.Amount - (debt.Amount + adjustmentsSum);

        LedgerEntry entry = new()
        {
            InternalReference = LedgerEntry.GenReferenceNumber,
            DebtId = debtId,
            Note = dto.Note,
            TransactionType = TransactionType.Adjustment,
            PaymentMethod = PaymentMethod.Adjustment,
            PaymentStatus = PaymentStatus.Confirmed,
            Amount = adjustmentAmount,
            CreatedByUserId = userId,
        };

        _context.LedgerEntries.Add(entry);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
