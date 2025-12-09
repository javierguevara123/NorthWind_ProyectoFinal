namespace NorthWind.Sales.Entities.Dtos.Orders.GetOrders
{
    public class GetOrdersQueryDto
    {
        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Filtros
        public string? CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        // Ordenamiento
        public string? OrderBy { get; set; } = "date";  // date, customer, amount
        public bool OrderDescending { get; set; } = true;

        public void Validate()
        {
            if (PageNumber < 1) PageNumber = 1;
            if (PageSize < 1) PageSize = 10;
            if (PageSize > 100) PageSize = 100;
        }
    }
}
