using NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;  // ← AGREGAR
using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;
using NorthWind.Sales.Entities.Dtos.Products.GetProducts;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories
{
    public interface IQueriesRepository
    {

        Task<OrderPagedResultDto> GetOrdersPaged(GetOrdersQueryDto query);

        // ========== PRODUCTS ==========
        Task<IEnumerable<ProductDto>> GetAllProducts();
        Task<ProductDto?> GetProductById(int productId);
        Task<bool> ProductExists(int productId);
        Task<PagedResultDto<ProductDto>> GetProductsPaged(GetProductsQueryDto query);
        Task<short> GetCommittedUnits(int productId);
        Task<bool> ProductNameExists(string name, int excludeProductId);
        Task<bool> ProductNameExists(string name);

        // ========== CUSTOMERS ==========
        Task<decimal?> GetCustomerCurrentBalance(string customerId);
        Task<bool> CustomerHasPendingOrders(string customerId);
        Task<CustomerPagedResultDto> GetCustomersPaged(GetCustomersQueryDto query);
        Task<CustomerDetailDto?> GetCustomerById(string customerId);
        Task<bool> CustomerExists(string customerId);
        Task<bool> CustomerNameExists(string name);
        Task<bool> CustomerNameExists(string name, string excludeCustomerId);

        // ========== PRODUCTS & CUSTOMERS (HELPERS) ==========
        Task<IEnumerable<ProductUnitsInStock>> GetProductsUnitsInStock(IEnumerable<int> productIds);

        // ========== ORDERS ========== ← AGREGAR ESTA SECCIÓN
        /// <summary>
        /// Obtiene una orden por ID con todos sus detalles.
        /// </summary>
        Task<OrderWithDetailsDto?> GetOrderById(int orderId);

        /// <summary>
        /// Verifica si una orden existe por su ID.
        /// </summary>
        Task<bool> OrderExists(int orderId);
    }
}