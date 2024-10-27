namespace Comanda.WebApi.Data.Mappings;

public sealed class CouponEntityConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");
        builder.HasKey(coupon => coupon.Id);

        builder.Property(coupon => coupon.Code)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(coupon => coupon.ExpirationDate)
            .IsRequired();

        builder.Property(coupon => coupon.Type)
            .IsRequired();

        builder.Property(coupon => coupon.Discount)
            .HasPrecision(9, 2);
    }
}