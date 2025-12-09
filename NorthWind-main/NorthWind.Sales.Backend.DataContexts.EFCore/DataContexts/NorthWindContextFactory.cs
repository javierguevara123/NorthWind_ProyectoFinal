using Microsoft.EntityFrameworkCore.Design;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts
{
    internal class NorthWindContextFactory : IDesignTimeDbContextFactory<NorthWindContext>
    {
        public NorthWindContext CreateDbContext(string[] args)
        {
            // Cadena de conexión para TIEMPO DE DISEÑO (Crear migraciones)
            // Asegúrate que apunte a NorthWindDB
            var connectionString = "Data Source=JAVIER;Initial Catalog=NorthWindDB;Integrated Security=True;Trust Server Certificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<NorthWindContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Pasamos null en IOptions porque en tiempo de diseño no hay inyección de dependencias,
            // pero el constructor base(options) ya tiene la configuración necesaria arriba.
            return new NorthWindContext(optionsBuilder.Options, null);
        }
    }
}