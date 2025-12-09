using NorthWind.DomainLogs.Entities.Dtos;
using NorthWind.DomainLogs.Entities.ValueObjects;
using NorthWind.Entities.Interfaces;

namespace NorthWind.DomainLogs.Entities.Interfaces
{
    public interface IDomainLogsRepository : IUnitOfWork
    {
        Task Add(DomainLog log);
        Task AddError(ErrorLog log);

        Task<PaginatedLogsDto<DomainLogDto>> GetDomainLogsPaged(int page, int pageSize);
        Task<PaginatedLogsDto<ErrorLogDto>> GetErrorLogsPaged(int page, int pageSize);
    }
}
