using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Sandwich)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(o => o.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.Discount)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(o => o.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.CreatedAt).IsRequired();
    }
}
