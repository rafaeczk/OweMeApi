using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal class DebtAdjustmentConfiguration : IEntityTypeConfiguration<DebtAdjustment>
{
    public void Configure(EntityTypeBuilder<DebtAdjustment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.OwnsOne(a => a.Money, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount");
        });
    }
}
