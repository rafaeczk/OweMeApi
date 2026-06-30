using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal class FriendCodeConfiguration : IEntityTypeConfiguration<FriendCode>
{
    public void Configure(EntityTypeBuilder<FriendCode> builder)
    {
        builder.HasKey(fc => fc.UserId);

        builder.Property(fc => fc.Code)
              .IsRequired()
              .HasMaxLength(6);

        builder.HasIndex(fc => fc.Code)
              .IsUnique();

        builder.HasOne<User>()
              .WithOne()
              .HasForeignKey<FriendCode>(fc => fc.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
