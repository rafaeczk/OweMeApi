using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class LedgerEventConfiguration : IEntityTypeConfiguration<LedgerEvent>
{
    public void Configure(EntityTypeBuilder<LedgerEvent> entity)
    {
        entity.HasIndex(e => e.InternalReference)
            .IsUnique();

        entity.HasOne(e => e.Debt)
            .WithMany(d => d.LedgerEvents)
            .HasForeignKey(e => e.DebtId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.ConfigureAuditableEntryFields();

        // DETAILS

        entity.HasOne(e => e.Adjustment)
            .WithOne(a => a.LedgerEvent)
            .HasForeignKey<LedgerEvent>(e => e.AdjustmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Payment)
            .WithOne(p => p.LedgerEvent)
            .HasForeignKey<LedgerEvent>(e => e.PaymentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.PaymentStatusChange)
            .WithOne(psc => psc.LedgerEvent)
            .HasForeignKey<LedgerEvent>(e => e.PaymentStatusChangeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
