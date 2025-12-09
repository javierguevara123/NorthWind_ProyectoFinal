namespace NorthWind.Sales.Backend.DataContexts.EFCore;

public static class DependencyContainer
{
    public static IServiceCollection AddDataContexts(this IServiceCollection services, Action<DBOptions> configureDBOptions)
    {
        // 1. Configura las opciones (trae el string de conexión del appsettings)
        services.Configure(configureDBOptions);

        // 2. --- ESTO ES LO QUE FALTABA ---
        // Registra el NorthWindContext unificado para que se puedan inyectar sus DbContextOptions
        services.AddDbContext<NorthWindContext>((serviceProvider, options) =>
        {
            var dbOptions = serviceProvider.GetRequiredService<IOptions<DBOptions>>().Value;
            options.UseSqlServer(dbOptions.ConnectionString);
        });

        // 3. Registra tus servicios específicos (que ahora dependen de lo de arriba)
        services.AddScoped<INorthWindSalesCommandsDataContext, NorthWindSalesCommandsDataContext>();
        services.AddScoped<INorthWindSalesQueriesDataContext, NorthWindSalesQueriesDataContext>();
        services.AddScoped<INorthWindDomainLogsDataContext, NorthWindDomainLogsDataContext>();

        return services;
    }
}