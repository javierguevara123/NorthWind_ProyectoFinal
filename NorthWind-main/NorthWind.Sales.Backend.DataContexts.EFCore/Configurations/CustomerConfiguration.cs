using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Configurations
{
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Id)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(c => c.CurrentBalance)
                .HasPrecision(8, 2);

            // --- Configuración de Nuevos Campos ---
            // Solo definimos longitudes para la BD
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.Cedula).HasMaxLength(20);
            builder.Property(c => c.HashedPassword).HasMaxLength(500); // Espacio suficiente para un hash

            // --- Datos Semilla Actualizados ---
            builder.HasData(
                new Customer
                {
                    Id = "ALFKI",
                    Name = "Alfreds Futterkiste",
                    CurrentBalance = 0,
                    Email = "alfreds@demo.com",
                    Cedula = "0000000001",
                    HashedPassword = "hash_demo_1"
                },
                new Customer
                {
                    Id = "ANATR",
                    Name = "Ana Trujillo Emparedados y helados",
                    CurrentBalance = 0,
                    Email = "ana@demo.com",
                    Cedula = "0000000002",
                    HashedPassword = "hash_demo_2"
                },
                new Customer
                {
                    Id = "ANTON",
                    Name = "Antonio Moreno Taquería",
                    CurrentBalance = 100,
                    Email = "antonio@demo.com",
                    Cedula = "0000000003",
                    HashedPassword = "hash_demo_3"
                }
            );
        }
    }
}