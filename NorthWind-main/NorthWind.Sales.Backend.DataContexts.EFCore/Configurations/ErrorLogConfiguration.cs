using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Configurations
{
    internal class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
    {
        public void Configure(EntityTypeBuilder<ErrorLog> builder)
        {
            builder.ToTable("ErrorLogs"); // Nombre de la tabla en BD

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(4000); // SQL Server nvarchar(4000)

            builder.Property(e => e.StackTrace)
                .HasMaxLength(8000); // O usa 'HasColumnType("nvarchar(max)")' si esperas traces muy largos

            builder.Property(e => e.Source)
                .HasMaxLength(100);

            builder.Property(e => e.User)
                .IsRequired()
                .HasMaxLength(100); // Ajustado al User.Identity.Name
        }
    }
}
