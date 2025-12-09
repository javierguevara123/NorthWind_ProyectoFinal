using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.Repositories.Interfaces
{
    public interface INorthWindSalesQueriesDataContext
    {
        // ========== DbSets ==========
        IQueryable<Customer> Customers { get; }
        IQueryable<Product> Products { get; }
        IQueryable<Order> Orders { get; }
        IQueryable<OrderDetail> OrderDetails { get; }

        // ========== Métodos Helper para Queries ==========
        Task<ReturnType> FirstOrDefaultAync<ReturnType>(IQueryable<ReturnType> queryable);
        Task<IEnumerable<ReturnType>> ToListAsync<ReturnType>(IQueryable<ReturnType> queryable);

        // ⬅️ AGREGAR estos métodos faltantes
        Task<int> CountAsync<T>(IQueryable<T> queryable);
        Task<bool> AnyAsync<T>(IQueryable<T> queryable);
        Task<int> SumAsync(IQueryable<int> queryable);  // Para sumas de enteros
        Task<long> SumAsync(IQueryable<long> queryable);  // Para sumas de long (opcional)
    }

}
