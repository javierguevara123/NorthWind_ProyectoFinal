using Microsoft.EntityFrameworkCore;
using NorthWind.DomainLogs.Entities.Dtos;
using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.Repositories.Repositories;

internal class DomainLogsRepository(INorthWindDomainLogsDataContext context) : IDomainLogsRepository
{
    public async Task Add(DomainLog log)
    {
        // CORRECCIÓN AQUÍ: Mapeo correcto de propiedades
        await context.AddAsync(new Entities.DomainLog
        {
            Information = log.Information,
            UserName = log.User,      // Antes decía 'User', debe ser 'UserName'
            CreatedDate = log.DateTime    // Antes decía 'DateTime', en la entidad es 'CreatedDate'
        });

        // Guardar cambios si la interfaz lo requiere
        await context.SaveChangesAsync();
    }

    public async Task AddError(ErrorLog log)
    {
        await context.AddAsync(new Entities.ErrorLog
        {
            Message = log.Message,
            StackTrace = log.StackTrace,
            Source = log.Source,
            User = log.User, // Verifica que en ErrorLog.cs (Entidad) la propiedad se llame 'User' y no 'UserName'
            Date = log.Date
        });
        await context.SaveChangesAsync();
    }

    public async Task<PaginatedLogsDto<DomainLogDto>> GetDomainLogsPaged(int page, int pageSize)
    {
        var query = context.DomainLogs.AsQueryable();

        // Contar total
        var totalCount = await query.CountAsync();

        // Obtener datos paginados y mapear a DTO
        var items = await query
            .OrderByDescending(x => x.CreatedDate) // Ordenar por fecha descendente
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new DomainLogDto(l.Id, l.Information, l.UserName, l.CreatedDate))
            .ToListAsync();

        return new PaginatedLogsDto<DomainLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedLogsDto<ErrorLogDto>> GetErrorLogsPaged(int page, int pageSize)
    {
        var query = context.ErrorLogs.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new ErrorLogDto(e.Id, e.Message, e.StackTrace, e.Source, e.User, e.Date))
            .ToListAsync();

        return new PaginatedLogsDto<ErrorLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    // Implementación de IUnitOfWork
    public async Task SaveChanges()
    {
        await context.SaveChangesAsync();
    }
}