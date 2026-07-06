using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal class LedgerEventConfiguration : IEntityTypeConfiguration<LedgerEvent>
{
    public void Configure(EntityTypeBuilder<LedgerEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.InternalReference)
            .IsUnique();

        builder.HasOne(e => e.Debt)
            .WithMany(d => d.LedgerEvents)
            .HasForeignKey(e => e.DebtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ConfigureAuditableEntryFields();

        builder.HasOne(e => e.Payment)
           .WithOne(p => p.LedgerEvent)
           .HasForeignKey<DebtPayment>(p => p.LedgerEventId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PaymentStatusChange)
            .WithOne(psc => psc.LedgerEvent)
            .HasForeignKey<DebtPaymentStatusChange>(psc => psc.LedgerEventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Adjustment)
            .WithOne(a => a.LedgerEvent)
            .HasForeignKey<DebtAdjustment>(a => a.LedgerEventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
