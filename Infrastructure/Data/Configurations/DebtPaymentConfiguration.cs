using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal class DebtPaymentConfiguration : IEntityTypeConfiguration<DebtPayment>
{
    public void Configure(EntityTypeBuilder<DebtPayment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.OwnsOne(a => a.Money, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount");
        });
    }
}