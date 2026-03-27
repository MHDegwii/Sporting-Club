using Microsoft.EntityFrameworkCore;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.Infrastructure.Services;

public sealed class EfCoreTicketService : ITicketService
{
    private readonly SportingClubDbContext _context;

    public EfCoreTicketService(SportingClubDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket> CreateAsync(CreateTicketRequest request, Guid createdBy)
    {
        var ticket = new Ticket
        {
            EventName = request.EventName,
            Quantity = request.Quantity,
            Price = request.Price,
            AvailableSinceUtc = request.AvailableSinceUtc,
            AvailableUntilUtc = request.AvailableUntilUtc,
            CreatedBy = createdBy
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<PagedResult<Ticket>> ListAsync(QueryOptions options)
    {
        var query = _context.Tickets.Where(x => x.AvailableUntilUtc > DateTime.UtcNow);

        // Search
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.EventName.Contains(options.Search));
        }

        var totalCount = await query.CountAsync();

        // Sorting
        query = (options.SortBy?.ToLower()) switch
        {
            "eventname" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.EventName) : query.OrderBy(x => x.EventName),
            "price" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
            "available" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.AvailableUntilUtc) : query.OrderBy(x => x.AvailableUntilUtc),
            _ => query.OrderBy(x => x.EventName)
        };

        // Pagination
        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PagedResult<Ticket>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Ticket?> GetAsync(Guid id)
    {
        return await _context.Tickets.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Ticket?> UpdateAsync(Guid id, UpdateTicketRequest request)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == id);
        if (ticket is null)
            return null;

        ticket.EventName = request.EventName;
        ticket.Quantity = request.Quantity;
        ticket.Price = request.Price;
        ticket.AvailableSinceUtc = request.AvailableSinceUtc;
        ticket.AvailableUntilUtc = request.AvailableUntilUtc;
        ticket.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == id);
        if (ticket is null)
            return false;

        // Delete associated bookings first
        var bookings = await _context.TicketBookings.Where(x => x.TicketId == id).ToListAsync();
        _context.TicketBookings.RemoveRange(bookings);

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TicketBooking> BookTicketAsync(BookTicketRequest request, Guid userId)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == request.TicketId);
        if (ticket is null)
            throw new InvalidOperationException("Ticket not found");

        if (ticket.AvailableUntilUtc < DateTime.UtcNow)
            throw new InvalidOperationException("Ticket is no longer available");

        // Check total booked quantity
        var totalBooked = await _context.TicketBookings
            .Where(x => x.TicketId == request.TicketId)
            .SumAsync(x => x.QuantityBooked);

        if (totalBooked + request.Quantity > ticket.Quantity)
            throw new InvalidOperationException("Not enough tickets available");

        var booking = new TicketBooking
        {
            TicketId = request.TicketId,
            UserId = userId,
            QuantityBooked = request.Quantity,
            BookedAtUtc = DateTime.UtcNow
        };

        _context.TicketBookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<PagedResult<TicketBooking>> ListUserBookingsAsync(Guid userId, QueryOptions options)
    {
        var query = _context.TicketBookings.Where(x => x.UserId == userId);

        var totalCount = await query.CountAsync();

        // Sorting
        query = (options.SortBy?.ToLower()) switch
        {
            "booked" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.BookedAtUtc) : query.OrderBy(x => x.BookedAtUtc),
            _ => query.OrderByDescending(x => x.BookedAtUtc)
        };

        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PagedResult<TicketBooking>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }
}
