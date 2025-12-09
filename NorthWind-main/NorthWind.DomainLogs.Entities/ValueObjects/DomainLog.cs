namespace NorthWind.DomainLogs.Entities.ValueObjects
{
    public class DomainLog(string information, string user)
    {
        public string Information => information;

        // Propiedad que faltaba (User)
        public string User => user;

        // Propiedad que faltaba (DateTime), se genera automáticamente
        public DateTime DateTime { get; } = DateTime.UtcNow;
    }

}
