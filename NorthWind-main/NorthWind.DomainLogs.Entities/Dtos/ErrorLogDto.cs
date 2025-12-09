namespace NorthWind.DomainLogs.Entities.Dtos
{
    public record ErrorLogDto(int Id, string Message, string StackTrace, string Source, string User, DateTime Date);
}
