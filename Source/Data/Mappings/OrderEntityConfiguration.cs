namespace Comanda.WebApi.Data.Mappings;

public sealed class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    private const string _tableName = "Orders";

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(order => order.Id);

        builder.Property(order => order.Customer)
            .IsRequired();

        builder.Property(order => order.ShippingAddress)
            .IsRequired();

        builder.Property(order => order.Date)
            .IsRequired();

        builder.Property(order => order.Status)
            .IsRequired();
    }
}