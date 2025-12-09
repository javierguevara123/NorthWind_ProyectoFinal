namespace NorthWind.Sales.Entities.Dtos.Products.GetProducts;

/// <summary>
/// DTO para parámetros de consulta de productos con paginación y filtros.
/// </summary>
public class GetProductsQueryDto
{
    // ========== PAGINACIÓN ==========
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // ========== FILTROS ==========
    public string? SearchTerm { get; set; }  // Buscar por nombre
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsLowStock { get; set; }  // true = solo productos con stock bajo

    // ========== ORDENAMIENTO ==========
    public string? OrderBy { get; set; } = "name";  // name, price, stock
    public bool OrderDescending { get; set; } = false;

    // Validar valores
    public void Validate()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100;  // Límite máximo
    }
}
