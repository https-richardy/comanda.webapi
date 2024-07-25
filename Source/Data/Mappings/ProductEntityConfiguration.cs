namespace Comanda.WebApi.Data.Mappings;

public sealed class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    private const string _tableName = "Products";

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(product => product.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(product => product.Price)
            .IsRequired()
            .HasPrecision(9, 2);

        builder.Property(product => product.ImagePath)
            .IsRequired();
    }
}