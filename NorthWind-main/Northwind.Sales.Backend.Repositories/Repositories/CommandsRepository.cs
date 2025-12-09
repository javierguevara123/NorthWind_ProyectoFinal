
using Microsoft.EntityFrameworkCore;
using NorthWind.Sales.Backend.BusinessObjects.Entities;
using System.Diagnostics;

using RepoEntities = NorthWind.Sales.Backend.Repositories.Entities;
using BusinessEntities = NorthWind.Sales.Backend.BusinessObjects.Entities;

namespace NorthWind.Sales.Backend.Repositories.Repositories;

internal class CommandsRepository(INorthWindSalesCommandsDataContext context) : ICommandsRepository
{
    public async Task CreateOrder(OrderAggregate order)
    {
        var sw = Stopwatch.StartNew();

        await context.AddOrderAsync(order);
        await context.AddOrderDetailsAsync(
            order.OrderDetails
            .Select(d => new Entities.OrderDetail
            {
                Order = order,
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice
            }).ToArray());

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo CreateOrder en CommandsRepository: {sw.ElapsedMilliseconds} ms");
    }

    public async Task<int> CreateProduct(Product product)
    {
        var sw = Stopwatch.StartNew();

        var productEntity = new Entities.Product
        {
            Name = product.Name,
            UnitPrice = product.UnitPrice,
            UnitsInStock = product.UnitsInStock,
        };

        await context.AddAsync(productEntity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo CreateProduct en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return productEntity.Id;
    }

    public Task UpdateProduct(Product product)
    {
        var sw = Stopwatch.StartNew();

        var productEntity = new Entities.Product
        {
            Id = product.Id,
            Name = product.Name,
            UnitPrice = product.UnitPrice,
            UnitsInStock = product.UnitsInStock,
        };

        context.Update(productEntity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo UpdateProduct en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return Task.CompletedTask;
    }

    public Task DeleteProduct(int productId)
    {
        var sw = Stopwatch.StartNew();

        var productEntity = new Entities.Product { Id = productId };

        context.Remove(productEntity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo DeleteProduct en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return Task.CompletedTask;
    }

    

    // CommandsRepository.cs (fragmento a sustituir o añadir)
    public async Task<string> CreateCustomer(Customer customer)
    {
        var sw = Stopwatch.StartNew();

        var entity = new Entities.Customer
        {
            Id = customer.Id,
            Name = customer.Name,
            CurrentBalance = customer.CurrentBalance
        };

        await context.AddAsync(entity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo CreateCustomer en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return entity.Id;
    }

    public Task UpdateCustomer(Customer customer)
    {
        var sw = Stopwatch.StartNew();

        var entity = new Entities.Customer
        {
            Id = customer.Id,
            Name = customer.Name,
            CurrentBalance = customer.CurrentBalance
        };

        context.Update(entity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo UpdateCustomer en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return Task.CompletedTask;
    }

    public Task DeleteCustomer(string customerId)
    {
        var sw = Stopwatch.StartNew();

        var entity = new Entities.Customer { Id = customerId };
        context.Remove(entity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo DeleteCustomer en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return Task.CompletedTask;
    }

    public Task DeleteOrder(int orderId)
    {
        var sw = Stopwatch.StartNew();

        var entity = new Order { Id = orderId };
        context.Remove(entity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo DeleteOrder en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return Task.CompletedTask;
    }

    public async Task<List<BusinessEntities.Product>> GetProductsWithLock(List<int> productIds)
    {
        if (productIds == null || !productIds.Any())
            return new List<BusinessEntities.Product>();

        var parameterNames = Enumerable.Range(0, productIds.Count).Select(i => $"{{{i}}}").ToArray();
        var sql = $"SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id IN ({string.Join(",", parameterNames)})";

        // CORRECCIÓN: Usar la entidad del repositorio (RepoEntities.Product)
        var efProducts = await context.Set<RepoEntities.Product>()
            .FromSqlRaw(sql, productIds.Cast<object>().ToArray())
            .ToListAsync();

        // Mapear de Entidad de Repositorio -> Entidad de Negocio
        return efProducts.Select(e => new BusinessEntities.Product
        {
            Id = e.Id,
            Name = e.Name,
            UnitsInStock = e.UnitsInStock,
            UnitPrice = e.UnitPrice
        }).ToList();
    }

    public Task UpdateProductStock(int productId, short newStock)
    {
        // Acceder al DbSet de la entidad de persistencia
        var dbSet = context.Set<RepoEntities.Product>();

        // Buscar en la caché local (debería estar ahí gracias a GetProductsWithLock)
        var entity = dbSet.Local.FirstOrDefault(e => e.Id == productId);

        // Si no está en memoria, crear un stub y adjuntarlo
        if (entity == null)
        {
            entity = new RepoEntities.Product { Id = productId };
            dbSet.Attach(entity);
        }

        // Actualizar el valor
        entity.UnitsInStock = newStock;

        // Forzar el estado a modificado para asegurar el UPDATE
        context.Update(entity);

        return Task.CompletedTask;
    }

    public async Task SaveChanges()
    {
        var sw = Stopwatch.StartNew();

        await context.SaveChangesAsync();

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo SaveChanges en CommandsRepository: {sw.ElapsedMilliseconds} ms");
    }
}


