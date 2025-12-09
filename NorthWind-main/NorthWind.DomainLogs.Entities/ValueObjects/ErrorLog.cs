namespace NorthWind.DomainLogs.Entities.ValueObjects
{
    public class ErrorLog(string message, string stackTrace, string source, string user)
    {
        public string Message => message;
        public string StackTrace => stackTrace;
        public string Source => source;
        public string User => user;
        public DateTime Date { get; } = DateTime.UtcNow;
    }
}
