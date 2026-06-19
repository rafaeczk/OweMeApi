using Domain.Entities;
using Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("AspNetUsers");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("Id");
        builder.Property(u => u.FullName).HasColumnName("FullName");
        builder.Property(u => u.Email).HasColumnName("Email");

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<User>(u => u.Id);
    }
}
