namespace Comanda.WebApi.Data.Mappings;

public sealed class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.PaymentIntentId)
            .IsRequired();

        builder.Property(payment => payment.Amount)
            .HasPrecision(9, 2);

        builder.Navigation(payment => payment.Order)
            .AutoInclude(true);
    }
}