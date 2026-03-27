using Microsoft.EntityFrameworkCore;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.Infrastructure.Services;

public sealed class EfCoreStoreService : IStoreService
{
    private readonly SportingClubDbContext _context;

    public EfCoreStoreService(SportingClubDbContext context)
    {
        _context = context;
    }

    public async Task<Store> CreateAsync(CreateStoreRequest request, Guid createdBy)
    {
        var store = new Store
        {
            Name = request.Name,
            Description = request.Description,
            BranchId = request.BranchId,
            CreatedBy = createdBy
        };

        _context.Stores.Add(store);
        await _context.SaveChangesAsync();
        return store;
    }

    public async Task<PagedResult<Store>> ListAsync(QueryOptions options)
    {
        var query = _context.Stores.AsQueryable();

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

        return new PagedResult<Store>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Store?> GetAsync(Guid id)
    {
        return await _context.Stores.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Store?> UpdateAsync(Guid id, UpdateStoreRequest request)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(x => x.Id == id);
        if (store is null)
            return null;

        store.Name = request.Name;
        store.Description = request.Description;
        store.BranchId = request.BranchId;
        store.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return store;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(x => x.Id == id);
        if (store is null)
            return false;

        // Delete associated offers first
        var offers = await _context.Offers.Where(x => x.StoreId == id).ToListAsync();
        _context.Offers.RemoveRange(offers);

        _context.Stores.Remove(store);
        await _context.SaveChangesAsync();
        return true;
    }
}
