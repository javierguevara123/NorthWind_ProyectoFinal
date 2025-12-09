using Microsoft.EntityFrameworkCore;
using NorthWind.Sales.Backend.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWind.Sales.Backend.Repositories.Interfaces
{
    public interface INorthWindDomainLogsDataContext
    {
        DbSet<DomainLog> DomainLogs { get; set; }

        // AGREGAR ESTO
        DbSet<ErrorLog> ErrorLogs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Métodos genéricos para AddAsync si los usas
        ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
    }

}
