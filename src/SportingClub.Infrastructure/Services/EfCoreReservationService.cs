using Microsoft.EntityFrameworkCore;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.Infrastructure.Services;

public sealed class EfCoreReservationService : IReservationService
{
    private readonly SportingClubDbContext _context;
    private const string SLOT_SEPARATOR = "-";
    private const string SLOT_FORMAT = "HH:mm";

    public EfCoreReservationService(SportingClubDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation> CreateAsync(CreateReservationRequest request)
    {
        // Check for conflicts
        var hasConflict = await CheckConflictAsync(request.SportId, request.ReservationDateUtc, request.TimeSlot);
        if (hasConflict)
            throw new InvalidOperationException("Time slot is already booked");

        var reservation = new Reservation
        {
            MemberId = request.MemberId,
            SportId = request.SportId,
            ReservationDateUtc = request.ReservationDateUtc,
            TimeSlot = request.TimeSlot,
            Status = "Confirmed",
            Notes = request.Notes
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task<PagedResult<Reservation>> ListAsync(QueryOptions options)
    {
        var query = _context.Reservations.AsQueryable();

        // Filter by status if specified
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.Status.Contains(options.Search));
        }

        var totalCount = await query.CountAsync();

        // Sorting
        query = (options.SortBy?.ToLower()) switch
        {
            "date" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.ReservationDateUtc) : query.OrderBy(x => x.ReservationDateUtc),
            "status" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "created" => options.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderBy(x => x.ReservationDateUtc)
        };

        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PagedResult<Reservation>
        {
            Items = items,
            Page = options.Page,
            PageSize = options.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Reservation?> GetAsync(Guid id)
    {
        return await _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Reservation?> UpdateAsync(Guid id, UpdateReservationRequest request)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);
        if (reservation is null)
            return null;

        // If changing time slot, check for conflicts
        if (reservation.TimeSlot != request.TimeSlot || reservation.ReservationDateUtc != request.ReservationDateUtc)
        {
            var hasConflict = await CheckConflictAsync(reservation.SportId, request.ReservationDateUtc, request.TimeSlot);
            if (hasConflict)
                throw new InvalidOperationException("Time slot is already booked");
        }

        reservation.ReservationDateUtc = request.ReservationDateUtc;
        reservation.TimeSlot = request.TimeSlot;
        reservation.Status = request.Status;
        reservation.Notes = request.Notes;
        reservation.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);
        if (reservation is null)
            return false;

        reservation.Status = "Cancelled";
        reservation.UpdatedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<string>> GetAvailableSlotsAsync(Guid sportId, DateTime date)
    {
        // Get all booked slots for this sport on this date
        var bookedSlots = await _context.Reservations
            .Where(x => x.SportId == sportId 
                && x.ReservationDateUtc.Date == date.Date 
                && x.Status == "Confirmed")
            .Select(x => x.TimeSlot)
            .ToListAsync();

        // Generate all possible slots (09:00-10:00, 10:00-11:00, etc.)
        var allSlots = GenerateTimeSlots();

        // Return available slots
        var availableSlots = allSlots.Except(bookedSlots).ToList();
        return availableSlots;
    }

    public async Task<bool> CheckConflictAsync(Guid sportId, DateTime date, string timeSlot)
    {
        // Check if this exact slot is already booked
        var conflict = await _context.Reservations
            .AnyAsync(x => x.SportId == sportId 
                && x.ReservationDateUtc.Date == date.Date 
                && x.TimeSlot == timeSlot 
                && x.Status == "Confirmed");

        return conflict;
    }

    /// <summary>
    /// Generate time slots for a day (09:00-10:00, 10:00-11:00, ... 17:00-18:00)
    /// </summary>
    private static List<string> GenerateTimeSlots()
    {
        var slots = new List<string>();

        for (int hour = 9; hour < 18; hour++)
        {
            var start = $"{hour:D2}:00";
            var end = $"{(hour + 1):D2}:00";
            slots.Add($"{start}-{end}");
        }

        return slots;
    }
}
