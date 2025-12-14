
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
            ProfilePicture = product.ProfilePicture
        };

        await context.AddAsync(productEntity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo CreateProduct en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return productEntity.Id;
    }

    public async Task UpdateProduct(Product product)
    {
        var existingEntity = await context.Set<RepoEntities.Product>().FindAsync(product.Id);

        if (existingEntity != null)
        {
            existingEntity.Name = product.Name;
            existingEntity.UnitsInStock = product.UnitsInStock;
            existingEntity.UnitPrice = product.UnitPrice;

            // Lógica de Imagen para Producto
            if (product.ProfilePicture != null && product.ProfilePicture.Length > 0)
            {
                existingEntity.ProfilePicture = product.ProfilePicture;
            }

            context.Update(existingEntity);
        }
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

        var entity = new RepoEntities.Customer
        {
            Id = customer.Id,
            Name = customer.Name,
            CurrentBalance = customer.CurrentBalance,
            Email = customer.Email,                 // Nuevo
            Cedula = customer.Cedula,               // Nuevo
            HashedPassword = customer.HashedPassword // Nuevo
        };

        await context.AddAsync(entity);

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo CreateCustomer en CommandsRepository: {sw.ElapsedMilliseconds} ms");

        return entity.Id;
    }

    public async Task UpdateCustomer(Customer customer)
    {
        var sw = Stopwatch.StartNew();

        // 1. Buscar la entidad existente para no perder la contraseña
        // Asumiendo que context hereda de DbContext o expone Set<T>
        var dbSet = context.Set<RepoEntities.Customer>();
        var existingEntity = await dbSet.FindAsync(customer.Id);

        if (existingEntity != null)
        {
            // 2. Actualizar solo los campos permitidos
            existingEntity.Name = customer.Name;
            existingEntity.CurrentBalance = customer.CurrentBalance;
            existingEntity.Email = customer.Email;
            existingEntity.Cedula = customer.Cedula;

            if (customer.ProfilePicture != null && customer.ProfilePicture.Length > 0)
            {
                existingEntity.ProfilePicture = customer.ProfilePicture;
            }

            if (!string.IsNullOrEmpty(customer.HashedPassword))
            {
                existingEntity.HashedPassword = customer.HashedPassword;
            }

            context.Update(existingEntity);
        }

        sw.Stop();
        Console.WriteLine($"🕒 Tiempo UpdateCustomer en CommandsRepository: {sw.ElapsedMilliseconds} ms");
    }

    // ... (El resto de métodos DeleteCustomer, SaveChanges, etc. se mantienen igual) ...

    public Task DeleteCustomer(string customerId)
    {
        var sw = Stopwatch.StartNew();
        var entity = new RepoEntities.Customer { Id = customerId };
        context.Remove(entity);
        sw.Stop();
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


