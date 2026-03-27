using Microsoft.EntityFrameworkCore;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.Infrastructure.Services;

public sealed class EfCoreServiceService : IServiceService
{
    private readonly SportingClubDbContext _context;

    public EfCoreServiceService(SportingClubDbContext context)
    {
        _context = context;
    }

    public async Task<Service> CreateAsync(CreateServiceRequest request)
    {
        var service = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
            BranchId = request.BranchId
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<PagedResult<Service>> ListAsync(QueryOptions options)
    {
        var query = _context.Services.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.Name.Contains(options.Search) || x.Description.Contains(options.Search));
        }

        var totalCount = await query.CountAsync();

        // Sorting
        query = (options.SortBy?.ToLower()) switch
        {
            "name" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "status" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "created" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderBy(x => x.Name)
        };

        // Pagination
        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PagedResult<Service>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Service?> GetAsync(Guid id)
    {
        return await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Service?> UpdateAsync(Guid id, UpdateServiceRequest request)
    {
        var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
        if (service is null)
            return null;

        service.Name = request.Name;
        service.Description = request.Description;
        service.Status = request.Status;
        service.BranchId = request.BranchId;
        service.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
        if (service is null)
            return false;

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }
}
