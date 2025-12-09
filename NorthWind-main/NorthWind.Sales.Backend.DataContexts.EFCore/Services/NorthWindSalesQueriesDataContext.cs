using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Services;

internal class NorthWindSalesQueriesDataContext :
    NorthWindContext, // Cambio aquí
    INorthWindSalesQueriesDataContext
{
    // Ajustamos el constructor para pasar lo que pide el padre (NorthWindContext)
    public NorthWindSalesQueriesDataContext(
        DbContextOptions<NorthWindContext> options,
        IOptions<DBOptions> dbOptions)
        : base(options, dbOptions)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    // ========== DbSets (Usamos los del padre unificado) ==========
    public new IQueryable<Product> Products => base.Products;
    public new IQueryable<Customer> Customers => base.Customers;
    public new IQueryable<Order> Orders => base.Orders;
    public new IQueryable<OrderDetail> OrderDetails => base.OrderDetails;

    // ========== Métodos Helper (existentes) ==========
    public Task<ReturnType> FirstOrDefaultAync<ReturnType>(IQueryable<ReturnType> queryable) =>
        queryable.FirstOrDefaultAsync();

    public async Task<IEnumerable<ReturnType>> ToListAsync<ReturnType>(IQueryable<ReturnType> queryable) =>
        await queryable.ToListAsync();

    // ========== Métodos Helper (NUEVOS) ==========

    public Task<int> CountAsync<T>(IQueryable<T> queryable) =>
        queryable.CountAsync();

    public Task<bool> AnyAsync<T>(IQueryable<T> queryable) =>
        queryable.AnyAsync();

    public Task<int> SumAsync(IQueryable<int> queryable) =>
        queryable.SumAsync();

    public Task<long> SumAsync(IQueryable<long> queryable) =>
        queryable.SumAsync();
}