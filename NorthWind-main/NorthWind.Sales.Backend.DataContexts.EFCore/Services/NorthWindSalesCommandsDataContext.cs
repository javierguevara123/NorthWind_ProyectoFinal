using NorthWind.Sales.Backend.DataContexts.EFCore.Guards;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Services;

internal class NorthWindSalesCommandsDataContext(
    DbContextOptions<NorthWindContext> options,
    IOptions<DBOptions> dbOptions)
    : NorthWindContext(options, dbOptions), INorthWindSalesCommandsDataContext
{

    public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        await base.Set<TEntity>().AddAsync(entity);
    }

    public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        await base.Set<TEntity>().AddRangeAsync(entities);
    }

    public new void Update<TEntity>(TEntity entity) where TEntity : class
    {
        base.Set<TEntity>().Update(entity);
    }

    public new void Remove<TEntity>(TEntity entity) where TEntity : class
    {
        // ✅ Attach + Estado Deleted explícito
        base.Attach(entity);
        base.Entry(entity).State = EntityState.Deleted;
    }

    //  Agrega un objeto "Order" al contexto para ser persistido en la BD.
    public async Task AddOrderAsync(Order order) => await AddAsync(order);

    //  Agrega múltiples "OrderDetail" al contexto.
    public async Task AddOrderDetailsAsync(
        IEnumerable<OrderDetail> orderDetails) => await AddRangeAsync(orderDetails);

    public new DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }

    public async Task SaveChangesAsync() => await GuardDBContext.AgainstSaveChangesErrorAsync(this);
}