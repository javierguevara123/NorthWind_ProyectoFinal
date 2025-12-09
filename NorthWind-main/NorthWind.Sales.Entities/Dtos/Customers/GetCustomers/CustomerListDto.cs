namespace NorthWind.Sales.Entities.Dtos.Customers.GetCustomers
{
    /// <summary>
    /// DTO de solo lectura para representar un cliente en listados.
    /// </summary>
    public class CustomerListDto(
        string customerId,
        string name,
        string? city,
        string? country)
    {
        public string CustomerId => customerId;
        public string Name => name;
        public string? City => city;
        public string? Country => country;

        // Propiedades calculadas opcionales
        public string Display => $"{name} ({country})";
    }
}
