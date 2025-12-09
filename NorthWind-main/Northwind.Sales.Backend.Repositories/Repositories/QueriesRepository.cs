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
            var queryable = context.Products
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice
                ));

            return await context.ToListAsync(queryable);
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

            var committedUnits = await context.SumAsync(queryable); // ⬅️ Llamar a través del context
            return (short)committedUnits;
        }

        public async Task<ProductDto?> GetProductById(int productId)
        {
            var queryable = context.Products
                .Where(p => p.Id == productId)
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice
                ));

            return await context.FirstOrDefaultAync(queryable);
        }

        // ========== PRODUCTS CON PAGINACIÓN ==========

        public async Task<PagedResultDto<ProductDto>> GetProductsPaged(GetProductsQueryDto query)
        {
            // 1. Crear query base
            var queryable = context.Products.AsQueryable();

            // 2. Aplicar filtros
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

            // 3. Obtener total de registros (USANDO MÉTODO DEL CONTEXTO)
            var totalCount = await context.CountAsync(queryable);  // ⬅️ CORREGIDO

            // 4. Aplicar ordenamiento
            queryable = ApplyOrdering(queryable, query.OrderBy, query.OrderDescending);

            // 5. Aplicar paginación y proyección
            var pagedQuery = queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice
                ));

            // 6. Ejecutar query (USANDO MÉTODO DEL CONTEXTO)
            var items = await context.ToListAsync(pagedQuery);  // ⬅️ YA ESTABA BIEN

            // 7. Retornar resultado paginado
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

                "name" or _ => descending
                    ? queryable.OrderByDescending(p => p.Name)
                    : queryable.OrderBy(p => p.Name)
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

        public async Task<decimal?> GetCustomerCurrentBalance(
       string customerId)
        {
            var Queryable = context.Customers
            .Where(c => c.Id == customerId)
            .Select(c => new { c.CurrentBalance });
            var Result = await context.FirstOrDefaultAync(Queryable);
            return Result?.CurrentBalance;
        }
        public async Task<IEnumerable<ProductUnitsInStock>>
       GetProductsUnitsInStock(IEnumerable<int> productIds)
        {
            var Queryable = context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new ProductUnitsInStock(
            p.Id, p.UnitsInStock));
            return await context.ToListAsync(Queryable);
        }

        // ========== CUSTOMERS ==========

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

        public async Task<CustomerDetailDto?> GetCustomerById(string customerId)
        {
            var queryable =
                from c in context.Customers
                where c.Id == customerId
                select new CustomerDetailDto(
                    c.Id,
                    c.Name,
                    c.CurrentBalance
                );

            return await context.FirstOrDefaultAync(queryable);
        }

        public async Task<CustomerPagedResultDto> GetCustomersPaged(GetCustomersQueryDto query)
        {
            var baseQuery = context.Customers.AsQueryable();

            // filtro opcional por nombre
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.ToLower();
                baseQuery = baseQuery.Where(c => c.Name.ToLower().Contains(term));
            }

            var totalRecords = await context.CountAsync(baseQuery);

            var ordered = query.OrderDescending
                ? baseQuery.OrderByDescending(c => c.Name)
                : baseQuery.OrderBy(c => c.Name);

            var pagedQuery = ordered
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new CustomerListItemDto(
                    c.Id,
                    c.Name,
                    c.CurrentBalance
                ));

            var customers = await context.ToListAsync(pagedQuery);

            return new CustomerPagedResultDto(customers, totalRecords);
        }

        public async Task<bool> CustomerNameExists(string name)
        {
            var queryable = context.Customers
                                   .Where(c => c.Name.ToLower() == name.ToLower());
            return await context.AnyAsync(queryable);
        }

        public async Task<bool> CustomerNameExists(string name, string excludeCustomerId)
        {
            var queryable = context.Customers
                                   .Where(c => c.Name.ToLower() == name.ToLower() &&
                                               c.Id != excludeCustomerId);
            return await context.AnyAsync(queryable);
        }

        public async Task<OrderWithDetailsDto?> GetOrderById(int orderId)
        {
            // 1. Query principal - obtener datos de la orden
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

            if (orderData == null)
                return null;

            // 2. Obtener los detalles (SIN od.Id porque no existe)
            var detailsQuery =
                from od in context.OrderDetails
                where od.OrderId == orderId
                join product in context.Products on od.ProductId equals product.Id
                select new OrderDetailItemDto(
                    od.ProductId,                      // ✅ 1
                    product.Name,                      // ✅ 2
                    od.Quantity,                       // ✅ 3
                    od.UnitPrice,                      // ✅ 4
                    od.Quantity * od.UnitPrice         // ✅ 5 - Subtotal
                );

            var details = await context.ToListAsync(detailsQuery);

            // 3. Calcular totales
            decimal totalAmount = details.Sum(d => d.Subtotal);
            int itemCount = details.Count();

            // 4. Construir DTO final
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
            // 1. Query base con totales calculados
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

            // 2. Aplicar filtros
            if (!string.IsNullOrWhiteSpace(query.CustomerId))
            {
                baseQuery = baseQuery.Where(x => x.CustomerId == query.CustomerId);
            }

            if (query.FromDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.OrderDate >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.OrderDate <= query.ToDate.Value);
            }

            if (query.MinAmount.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.TotalAmount >= query.MinAmount.Value);
            }

            if (query.MaxAmount.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.TotalAmount <= query.MaxAmount.Value);
            }

            // 3. Contar total
            var totalCount = await context.CountAsync(baseQuery);

            // 4. Aplicar ordenamiento
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

            // 5. Aplicar paginación
            var pagedQuery = baseQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            // 6. Ejecutar y mapear
            var ordersData = await context.ToListAsync(pagedQuery);

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
