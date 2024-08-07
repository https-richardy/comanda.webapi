namespace Comanda.WebApi.Data.Mappings;

public sealed class AddressEntityConfiguration : IEntityTypeConfiguration<Address>
{
    private const string _tableName = "Addresses";

    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(address => address.Id);

        builder.Property(address => address.Street)
            .IsRequired()
            .HasMaxLength(160);

        builder.Property(address => address.Number)
            .HasDefaultValue("S/N")
            .HasMaxLength(8);

        builder.Property(address => address.City)
            .IsRequired()
            .HasMaxLength(160);

        builder.Property(address => address.State)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(address => address.Neighborhood)
            .IsRequired();

        builder.Property(address => address.PostalCode)
            .HasMaxLength(9)
            .IsRequired();

        builder.Property(address => address.Complement)
            .HasMaxLength(160);

        builder.Property(address => address.Reference)
            .HasMaxLength(160);
    }
}