using Microsoft.EntityFrameworkCore;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.Infrastructure.Services;

public sealed class EfCoreOfferService : IOfferService
{
    private readonly SportingClubDbContext _context;

    public EfCoreOfferService(SportingClubDbContext context)
    {
        _context = context;
    }

    public async Task<Offer> CreateAsync(CreateOfferRequest request)
    {
        var offer = new Offer
        {
            StoreId = request.StoreId,
            Description = request.Description,
            DiscountPercentage = request.DiscountPercentage,
            StartDateUtc = request.StartDateUtc,
            EndDateUtc = request.EndDateUtc
        };

        _context.Offers.Add(offer);
        await _context.SaveChangesAsync();
        return offer;
    }

    public async Task<PagedResult<Offer>> ListAsync(QueryOptions options)
    {
        var query = _context.Offers.AsQueryable();

        // Search by description or store
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.Description.Contains(options.Search));
        }

        var totalCount = await query.CountAsync();

        // Sorting
        query = (options.SortBy?.ToLower()) switch
        {
            "discount" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.DiscountPercentage) : query.OrderBy(x => x.DiscountPercentage),
            "enddate" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.EndDateUtc) : query.OrderBy(x => x.EndDateUtc),
            "created" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderBy(x => x.EndDateUtc)
        };

        // Pagination
        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PagedResult<Offer>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Offer?> GetAsync(Guid id)
    {
        return await _context.Offers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Offer?> UpdateAsync(Guid id, UpdateOfferRequest request)
    {
        var offer = await _context.Offers.FirstOrDefaultAsync(x => x.Id == id);
        if (offer is null)
            return null;

        offer.Description = request.Description;
        offer.DiscountPercentage = request.DiscountPercentage;
        offer.StartDateUtc = request.StartDateUtc;
        offer.EndDateUtc = request.EndDateUtc;
        offer.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return offer;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var offer = await _context.Offers.FirstOrDefaultAsync(x => x.Id == id);
        if (offer is null)
            return false;

        _context.Offers.Remove(offer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<Offer>> ListActiveOffersAsync(QueryOptions options)
    {
        var now = DateTime.UtcNow;
        var query = _context.Offers
            .Where(x => x.StartDateUtc <= now && x.EndDateUtc >= now);

        // Search
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.Description.Contains(options.Search));
        }

        var totalCount = await query.CountAsync();

        // Sorting
        query = (options.SortBy?.ToLower()) switch
        {
            "discount" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.DiscountPercentage) : query.OrderBy(x => x.DiscountPercentage),
            "enddate" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.EndDateUtc) : query.OrderBy(x => x.EndDateUtc),
            _ => query.OrderBy(x => x.EndDateUtc)
        };

        // Pagination
        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PagedResult<Offer>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }
}
