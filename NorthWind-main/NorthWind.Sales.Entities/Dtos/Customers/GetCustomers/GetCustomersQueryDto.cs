namespace NorthWind.Sales.Entities.Dtos.Customers.GetCustomers
{
    /// <summary>
    /// DTO para parámetros de consulta de clientes con paginación y filtros.
    /// </summary>
    public class GetCustomersQueryDto
    {
        // ========== PAGINACIÓN ==========
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // ========== FILTROS ==========
        public string? SearchTerm { get; set; }     // Buscar por nombre, ciudad, etc.
        public string? City { get; set; }
        public string? Country { get; set; }

        // ========== ORDENAMIENTO ==========
        // name, city, country
        public string? OrderBy { get; set; } = "name";
        public bool OrderDescending { get; set; } = false;

        // Validaciones
        public void Validate()
        {
            if (PageNumber < 1) PageNumber = 1;
            if (PageSize < 1) PageSize = 10;
            if (PageSize > 100) PageSize = 100;
        }
    }
}
