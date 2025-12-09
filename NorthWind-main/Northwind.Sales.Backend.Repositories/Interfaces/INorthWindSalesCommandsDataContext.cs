
using Microsoft.EntityFrameworkCore;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.Repositories.Interfaces;

public interface INorthWindSalesCommandsDataContext
{

    // ========== Métodos genéricos para CRUD ==========
    Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
    Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    void Update<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;

    // ========== Métodos legacy (Orders - mantener compatibilidad) ==========
    Task AddOrderAsync(Order order);
    Task AddOrderDetailsAsync(IEnumerable<OrderDetail> orderDetails);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    // ========== Save ==========
    Task SaveChangesAsync();
}