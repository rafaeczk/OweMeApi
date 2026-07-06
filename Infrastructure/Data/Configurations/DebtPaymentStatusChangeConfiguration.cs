using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal class DebtPaymentStatusChangeConfiguration : IEntityTypeConfiguration<DebtPaymentStatusChange>
{
    public void Configure(EntityTypeBuilder<DebtPaymentStatusChange> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
