using Microsoft.EntityFrameworkCore;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.Infrastructure.Services;

public sealed class EfCoreSportService : ISportService
{
    private readonly SportingClubDbContext _context;

    public EfCoreSportService(SportingClubDbContext context)
    {
        _context = context;
    }

    public async Task<Sport> CreateAsync(CreateSportRequest request)
    {
        var sport = new Sport
        {
            Name = request.Name,
            Description = request.Description,
            Schedule = request.Schedule,
            CoachId = request.CoachId,
            BranchId = request.BranchId
        };

        _context.Sports.Add(sport);
        await _context.SaveChangesAsync();
        return sport;
    }

    public async Task<PagedResult<Sport>> ListAsync(QueryOptions options)
    {
        var query = _context.Sports.AsQueryable();

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
            "created" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderBy(x => x.Name)
        };

        // Pagination
        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PagedResult<Sport>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Sport?> GetAsync(Guid id)
    {
        return await _context.Sports.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Sport?> UpdateAsync(Guid id, UpdateSportRequest request)
    {
        var sport = await _context.Sports.FirstOrDefaultAsync(x => x.Id == id);
        if (sport is null)
            return null;

        sport.Name = request.Name;
        sport.Description = request.Description;
        sport.Schedule = request.Schedule;
        sport.CoachId = request.CoachId;
        sport.BranchId = request.BranchId;
        sport.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return sport;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sport = await _context.Sports.FirstOrDefaultAsync(x => x.Id == id);
        if (sport is null)
            return false;

        _context.Sports.Remove(sport);
        await _context.SaveChangesAsync();
        return true;
    }
}
