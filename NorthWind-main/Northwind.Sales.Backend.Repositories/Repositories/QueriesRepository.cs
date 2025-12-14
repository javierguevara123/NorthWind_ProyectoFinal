using Microsoft.EntityFrameworkCore;
using NorthWind.Sales.Backend.BusinessObjects.ValueObjects;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;
using NorthWind.Sales.Entities.Dtos.Products.GetProducts;

namespace NorthWind.Sales.Backend.Repositories.Repositories
{
    internal class QueriesRepository(INorthWindSalesQueriesDataContext context) : IQueriesRepository
    {

        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            // 1. Traer datos crudos (incluyendo binario)
            var rawData = await context.Products
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice,
                    p.ProfilePicture // Asumiendo que agregaste este campo byte[] a la entidad Product
                })
                .ToListAsync();

            // 2. Convertir a DTO en memoria
            return rawData.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.UnitsInStock,
                p.UnitPrice,
                p.ProfilePicture != null ? Convert.ToBase64String(p.ProfilePicture) : null // <--- Conversión
            ));
        }

        public async Task<bool> ProductExists(int productId)
        {
            var queryable = context.Products.Where(p => p.Id == productId);
            return await context.AnyAsync(queryable);
        }

        public async Task<short> GetCommittedUnits(int productId)
        {
            var queryable = context.OrderDetails
                .Where(od => od.ProductId == productId)
                .Select(od => (int)od.Quantity);

            var committedUnits = await context.SumAsync(queryable);
            return (short)committedUnits;
        }

        public async Task<ProductDto?> GetProductById(int productId)
        {
            // 1. Traer proyección anónima
            var rawProduct = await context.Products
                .Where(p => p.Id == productId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice,
                    p.ProfilePicture // Campo binario
                })
                .FirstOrDefaultAsync();

            if (rawProduct == null) return null;

            // 2. Mapear y convertir
            return new ProductDto(
                rawProduct.Id,
                rawProduct.Name,
                rawProduct.UnitsInStock,
                rawProduct.UnitPrice,
                rawProduct.ProfilePicture != null ? Convert.ToBase64String(rawProduct.ProfilePicture) : null
            );
        }

        // ========== PRODUCTS CON PAGINACIÓN ==========

        public async Task<PagedResultDto<ProductDto>> GetProductsPaged(GetProductsQueryDto query)
        {
            var queryable = context.Products.AsQueryable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                queryable = queryable.Where(p => p.Name.ToLower().Contains(searchTerm));
            }

            if (query.MinPrice.HasValue)
                queryable = queryable.Where(p => p.UnitPrice >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                queryable = queryable.Where(p => p.UnitPrice <= query.MaxPrice.Value);

            if (query.IsLowStock.HasValue && query.IsLowStock.Value)
                queryable = queryable.Where(p => p.UnitsInStock < 10);

            var totalCount = await context.CountAsync(queryable);

            queryable = ApplyOrdering(queryable, query.OrderBy, query.OrderDescending);

            // PAGINACIÓN Y SELECCIÓN DE DATOS CRUDOS
            // Nota: No convertimos a DTO aquí para evitar error de traducción SQL con Base64
            var rawItems = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice,
                    p.ProfilePicture // Traemos los bytes
                })
                .ToListAsync();

            // CONVERSIÓN EN MEMORIA
            var items = rawItems.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.UnitsInStock,
                p.UnitPrice,
                p.ProfilePicture != null ? Convert.ToBase64String(p.ProfilePicture) : null
            )).ToList();

            return new PagedResultDto<ProductDto>
            {
                Items = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        private IQueryable<Entities.Product> ApplyOrdering(
            IQueryable<Entities.Product> queryable,
            string? orderBy,
            bool descending)
        {
            return orderBy?.ToLower() switch
            {
                "price" => descending
                    ? queryable.OrderByDescending(p => p.UnitPrice)
                    : queryable.OrderBy(p => p.UnitPrice),

                "stock" => descending
                    ? queryable.OrderByDescending(p => p.UnitsInStock)
                    : queryable.OrderBy(p => p.UnitsInStock),

                "name" => descending
                    ? queryable.OrderByDescending(p => p.Name)
                    : queryable.OrderBy(p => p.Name),

                "id" or _ => descending
                    ? queryable.OrderByDescending(p => p.Id)
                    : queryable.OrderBy(p => p.Id)
            };
        }

        public async Task<bool> ProductNameExists(string name)
        {
            var queryable = context.Products
                .Where(p => p.Name.ToLower() == name.ToLower());

            return await context.AnyAsync(queryable);
        }

        public async Task<bool> ProductNameExists(string name, int excludeProductId)
        {
            var queryable = context.Products
                .Where(p => p.Name.ToLower() == name.ToLower() && p.Id != excludeProductId);

            return await context.AnyAsync(queryable);
        }

        public async Task<decimal?> GetCustomerCurrentBalance(string customerId)
        {
            var Queryable = context.Customers
            .Where(c => c.Id == customerId)
            .Select(c => new { c.CurrentBalance });
            var Result = await context.FirstOrDefaultAync(Queryable);
            return Result?.CurrentBalance;
        }

        public async Task<IEnumerable<ProductUnitsInStock>> GetProductsUnitsInStock(IEnumerable<int> productIds)
        {
            var Queryable = context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new ProductUnitsInStock(p.Id, p.UnitsInStock));

            return await context.ToListAsync(Queryable);
        }

        public async Task<bool> CustomerExists(string customerId)
        {
            var queryable = context.Customers.Where(c => c.Id == customerId);
            return await context.AnyAsync(queryable);
        }

        public async Task<bool> CustomerHasPendingOrders(string customerId)
        {
            var balance = await GetCustomerCurrentBalance(customerId);
            return balance.HasValue && balance.Value > 0;
        }

        // ========== CUSTOMERS ==========

        public async Task<CustomerDetailDto?> GetCustomerById(string customerId)
        {
            // 1. Proyección anónima para traer el binario
            var rawCustomer = await context.Customers
                .Where(c => c.Id == customerId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.CurrentBalance,
                    c.Email,
                    c.Cedula,
                    c.ProfilePicture // Campo byte[]
                })
                .FirstOrDefaultAsync();

            if (rawCustomer == null) return null;

            // 2. Convertir a Base64 en memoria
            return new CustomerDetailDto(
                rawCustomer.Id,
                rawCustomer.Name,
                rawCustomer.CurrentBalance,
                rawCustomer.Email,
                rawCustomer.Cedula,
                rawCustomer.ProfilePicture != null ? Convert.ToBase64String(rawCustomer.ProfilePicture) : null
            );
        }

        public async Task<CustomerPagedResultDto> GetCustomersPaged(GetCustomersQueryDto query)
        {
            var baseQuery = context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.ToLower();
                baseQuery = baseQuery.Where(c => c.Name.ToLower().Contains(term) ||
                                                 c.Email.ToLower().Contains(term));
            }

            var totalRecords = await context.CountAsync(baseQuery);

            var ordered = query.OrderDescending
                ? baseQuery.OrderByDescending(c => c.Id)
                : baseQuery.OrderBy(c => c.Id);

            // 1. Traer datos crudos (sin DTO todavía)
            var rawItems = await ordered
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.CurrentBalance,
                    c.Email,
                    c.Cedula,
                    c.ProfilePicture // Binario
                })
                .ToListAsync();

            // 2. Mapear a DTO con conversión
            var customers = rawItems.Select(c => new CustomerListItemDto(
                c.Id,
                c.Name,
                c.CurrentBalance,
                c.Email,
                c.Cedula,
                c.ProfilePicture != null ? Convert.ToBase64String(c.ProfilePicture) : null // <--- Conversión
            )).ToList();

            return new CustomerPagedResultDto(customers, totalRecords);
        }

        public async Task<bool> CustomerNameExists(string name)
        {
            var queryable = context.Customers.Where(c => c.Name.ToLower() == name.ToLower());
            return await context.AnyAsync(queryable);
        }

        public async Task<bool> CustomerNameExists(string name, string excludeCustomerId)
        {
            var queryable = context.Customers
                .Where(c => c.Name.ToLower() == name.ToLower() && c.Id != excludeCustomerId);
            return await context.AnyAsync(queryable);
        }

        // ========== ORDERS (Sin cambios mayores, solo validando) ==========

        public async Task<OrderWithDetailsDto?> GetOrderById(int orderId)
        {
            // Query principal
            var queryable =
                from order in context.Orders
                where order.Id == orderId
                join customer in context.Customers on order.CustomerId equals customer.Id
                select new
                {
                    order.Id,
                    order.CustomerId,
                    CustomerName = customer.Name,
                    order.OrderDate,
                    order.ShipAddress,
                    order.ShipCity,
                    order.ShipCountry,
                    order.ShipPostalCode
                };

            var orderData = await context.FirstOrDefaultAync(queryable);

            if (orderData == null) return null;

            // Query detalles
            var detailsQuery =
                from od in context.OrderDetails
                where od.OrderId == orderId
                join product in context.Products on od.ProductId equals product.Id
                select new OrderDetailItemDto(
                    od.ProductId,
                    product.Name,
                    od.Quantity,
                    od.UnitPrice,
                    od.Quantity * od.UnitPrice
                );

            var details = await context.ToListAsync(detailsQuery);

            decimal totalAmount = details.Sum(d => d.Subtotal);
            int itemCount = details.Count();

            return new OrderWithDetailsDto(
                orderData.Id,
                orderData.CustomerId,
                orderData.CustomerName,
                orderData.OrderDate,
                orderData.ShipAddress,
                orderData.ShipCity,
                orderData.ShipCountry,
                orderData.ShipPostalCode,
                totalAmount,
                itemCount,
                details
            );
        }

        public async Task<bool> OrderExists(int orderId)
        {
            var queryable = context.Orders.Where(o => o.Id == orderId);
            return await context.AnyAsync(queryable);
        }

        public async Task<OrderPagedResultDto> GetOrdersPaged(GetOrdersQueryDto query)
        {
            // Query base
            var baseQuery =
                from order in context.Orders
                join customer in context.Customers on order.CustomerId equals customer.Id
                let totalAmount = context.OrderDetails
                    .Where(od => od.OrderId == order.Id)
                    .Sum(od => od.Quantity * od.UnitPrice)
                let itemCount = context.OrderDetails
                    .Where(od => od.OrderId == order.Id)
                    .Count()
                select new
                {
                    order.Id,
                    order.CustomerId,
                    CustomerName = customer.Name,
                    order.OrderDate,
                    order.ShipCity,
                    order.ShipCountry,
                    TotalAmount = totalAmount,
                    ItemCount = itemCount
                };

            // Filtros
            if (!string.IsNullOrWhiteSpace(query.CustomerId))
                baseQuery = baseQuery.Where(x => x.CustomerId == query.CustomerId);

            if (query.FromDate.HasValue)
                baseQuery = baseQuery.Where(x => x.OrderDate >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                baseQuery = baseQuery.Where(x => x.OrderDate <= query.ToDate.Value);

            if (query.MinAmount.HasValue)
                baseQuery = baseQuery.Where(x => x.TotalAmount >= query.MinAmount.Value);

            if (query.MaxAmount.HasValue)
                baseQuery = baseQuery.Where(x => x.TotalAmount <= query.MaxAmount.Value);

            // Contar
            var totalCount = await context.CountAsync(baseQuery);

            // Ordenamiento
            baseQuery = query.OrderBy?.ToLower() switch
            {
                "customer" => query.OrderDescending
                    ? baseQuery.OrderByDescending(x => x.CustomerName)
                    : baseQuery.OrderBy(x => x.CustomerName),
                "amount" => query.OrderDescending
                    ? baseQuery.OrderByDescending(x => x.TotalAmount)
                    : baseQuery.OrderBy(x => x.TotalAmount),
                "date" or _ => query.OrderDescending
                    ? baseQuery.OrderByDescending(x => x.OrderDate)
                    : baseQuery.OrderBy(x => x.OrderDate)
            };

            // Paginación
            var pagedQuery = baseQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            // Ejecución
            var ordersData = await context.ToListAsync(pagedQuery);

            // Mapeo
            var items = ordersData.Select(o => new OrderListItemDto(
                o.Id,
                o.CustomerId,
                o.CustomerName,
                o.OrderDate,
                o.ShipCity,
                o.ShipCountry,
                o.TotalAmount,
                o.ItemCount
            ));

            return new OrderPagedResultDto
            {
                Items = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }
    }
}