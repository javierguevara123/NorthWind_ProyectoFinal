namespace NorthWind.DomainLogs.Entities.Dtos
{
    public class PaginatedLogsDto<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
