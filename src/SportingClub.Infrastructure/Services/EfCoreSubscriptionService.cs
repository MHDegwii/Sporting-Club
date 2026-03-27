using Microsoft.EntityFrameworkCore;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.Infrastructure.Services;

public sealed class EfCoreSubscriptionService : ISubscriptionService
{
    private readonly SportingClubDbContext _context;

    public EfCoreSubscriptionService(SportingClubDbContext context)
    {
        _context = context;
    }

    public async Task<Subscription> CreateAsync(CreateSubscriptionRequest request)
    {
        var subscription = new Subscription
        {
            Name = request.Name,
            Price = request.Price,
            DurationDays = request.DurationDays,
            LinkedSportIds = request.LinkedSportIds
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task<PagedResult<Subscription>> ListAsync(QueryOptions options)
    {
        var query = _context.Subscriptions.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.Name.Contains(options.Search));
        }

        var totalCount = await query.CountAsync();

        // Sorting
        query = (options.SortBy?.ToLower()) switch
        {
            "name" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "price" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
            "duration" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.DurationDays) : query.OrderBy(x => x.DurationDays),
            "created" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderBy(x => x.Name)
        };

        // Pagination
        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PagedResult<Subscription>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Subscription?> GetAsync(Guid id)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Subscription?> UpdateAsync(Guid id, UpdateSubscriptionRequest request)
    {
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(x => x.Id == id);
        if (subscription is null)
            return null;

        subscription.Name = request.Name;
        subscription.Price = request.Price;
        subscription.DurationDays = request.DurationDays;
        subscription.LinkedSportIds = request.LinkedSportIds;
        subscription.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(x => x.Id == id);
        if (subscription is null)
            return false;

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return true;
    }
}
