namespace NorthWind.DomainLogs.Entities.Dtos
{
    public record DomainLogDto(int Id, string Information, string User, DateTime DateTime);
}
