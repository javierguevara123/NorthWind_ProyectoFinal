using System.Security.Cryptography; // Necesario para el hash
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Configurations
{
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Id).HasMaxLength(10).IsFixedLength();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(40);
            builder.Property(c => c.CurrentBalance).HasPrecision(8, 2);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.Cedula).HasMaxLength(20);
            builder.Property(c => c.HashedPassword).HasMaxLength(500);
            builder.Property(c => c.ProfilePicture).IsRequired(false);

            // --- Datos Semilla con Contraseñas Reales Hasheadas ---
            // Asumimos que la contraseña por defecto es "123456"
            string defaultPasswordHash = HashPassword("123456");

            builder.HasData(
                new Customer
                {
                    Id = "ALFKI",
                    Name = "Alfreds Futterkiste",
                    CurrentBalance = 0,
                    Email = "alfreds@demo.com",
                    Cedula = "0000000001",
                    HashedPassword = defaultPasswordHash // Hash real
                },
                new Customer
                {
                    Id = "ANATR",
                    Name = "Ana Trujillo Emparedados",
                    CurrentBalance = 0,
                    Email = "ana@demo.com",
                    Cedula = "0000000002",
                    HashedPassword = defaultPasswordHash // Hash real
                },
                new Customer
                {
                    Id = "ANTON",
                    Name = "Antonio Moreno Taquería",
                    CurrentBalance = 100,
                    Email = "antonio@demo.com",
                    Cedula = "0000000003",
                    HashedPassword = defaultPasswordHash // Hash real
                }
            );
        }

        // Método auxiliar para generar el hash dentro de la configuración
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}