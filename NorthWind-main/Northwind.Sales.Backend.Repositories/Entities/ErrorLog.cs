namespace NorthWind.Sales.Backend.Repositories.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
    }
}
