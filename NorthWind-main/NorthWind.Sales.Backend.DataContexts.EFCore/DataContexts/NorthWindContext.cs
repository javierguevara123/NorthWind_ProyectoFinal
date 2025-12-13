using NorthWind.Sales.Backend.DataContexts.EFCore.Configurations;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts;

// Unificamos todo en una sola clase
internal class NorthWindContext : DbContext
{
    private readonly IOptions<DBOptions> _dbOptions;

    // Constructor para Inyección de Dependencias (Runtime)
    public NorthWindContext(DbContextOptions<NorthWindContext> options, IOptions<DBOptions> dbOptions)
        : base(options)
    {
        _dbOptions = dbOptions;
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; } 

    // --- TABLAS DE LOGS (Agregadas aquí) ---
    public DbSet<DomainLog> DomainLogs { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Solo configuramos si no está configurado (útil para pruebas o diseño)
        if (!optionsBuilder.IsConfigured && _dbOptions != null)
        {
            optionsBuilder.UseSqlServer(_dbOptions.Value.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // APLICAR CONFIGURACIONES DE NEGOCIO
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ErrorLogConfiguration());

    }
}