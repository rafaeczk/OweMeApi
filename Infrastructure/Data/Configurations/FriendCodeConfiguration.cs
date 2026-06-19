using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class FriendCodeConfiguration : IEntityTypeConfiguration<FriendCode>
{
    public void Configure(EntityTypeBuilder<FriendCode> entity)
    {
        entity.HasKey(fc => fc.UserId);

        entity.Property(fc => fc.Code)
              .IsRequired()
              .HasMaxLength(6);

        entity.HasIndex(fc => fc.Code)
              .IsUnique();

        entity.HasOne<User>()
              .WithOne()
              .HasForeignKey<FriendCode>(fc => fc.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
