using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> entity)
    {
        entity.HasKey(f => new { f.UserId, f.FriendId });

        entity.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(f => f.Friend)
            .WithMany()
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
