using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Configurations;

internal class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        // 1. Llave primaria compuesta (OrderId + ProductId)
        builder.HasKey(d => new { d.OrderId, d.ProductId });

        // 2. Configuración de propiedades
        builder.Property(d => d.UnitPrice)
               .HasPrecision(8, 2);

        // 3. Relación con Order
        builder.HasOne(d => d.Order)
               .WithMany()
               .HasForeignKey(d => d.OrderId);

        // 4. Relación con Product (AQUÍ ESTÁ LA SOLUCIÓN)
        // Debes especificar explícitamente la propiedad de navegación (d => d.Product)
        // para que EF Core sepa que 'Product' y 'ProductId' van juntos.
        builder.HasOne(d => d.Product)
               .WithMany()
               .HasForeignKey(d => d.ProductId)
               .IsRequired();
    }
}