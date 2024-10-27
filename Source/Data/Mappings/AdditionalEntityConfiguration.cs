namespace Comanda.WebApi.Data.Mappings;

public sealed class AdditionalEntityConfiguration : IEntityTypeConfiguration<Additional>
{
    public void Configure(EntityTypeBuilder<Additional> builder)
    {
        builder.ToTable("Additionals");
        builder.HasKey(additional => additional.Id);

        builder.Property(additional => additional.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(additional => additional.Price)
            .IsRequired()
            .HasPrecision(9, 2);
    }
}