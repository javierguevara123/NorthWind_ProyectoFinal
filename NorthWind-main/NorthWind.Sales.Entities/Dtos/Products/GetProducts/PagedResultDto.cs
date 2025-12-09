namespace NorthWind.Sales.Entities.Dtos.Products.GetProducts;

/// <summary>
/// DTO genérico para resultados paginados.
/// Contiene los datos y metadatos de paginación.
/// </summary>
public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
