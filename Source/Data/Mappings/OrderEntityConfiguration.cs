namespace Comanda.WebApi.Data.Mappings;

public sealed class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    private const string _tableName = "Orders";

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(order => order.Id);

        builder.Property(order => order.Date)
            .IsRequired();

        builder.Property(order => order.Status)
            .IsRequired();


        builder.HasOne(order => order.Customer);
        builder.HasOne(order => order.ShippingAddress);
    }
}