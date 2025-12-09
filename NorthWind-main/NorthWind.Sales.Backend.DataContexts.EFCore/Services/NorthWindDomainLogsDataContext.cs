using NorthWind.Sales.Backend.DataContexts.EFCore.Guards;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Services;

internal class NorthWindDomainLogsDataContext(
    DbContextOptions<NorthWindContext> options,
    IOptions<DBOptions> dbOptions)
    : NorthWindContext(options, dbOptions), INorthWindDomainLogsDataContext
{
    public async Task AddLogAsync(DomainLog log) =>
        await DomainLogs.AddAsync(log); // Accedemos directamente al DbSet del contexto unificado

    public async Task SaveChangesAsync() =>
        await GuardDBContext.AgainstSaveChangesErrorAsync(this);
}