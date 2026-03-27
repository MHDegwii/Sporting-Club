namespace SportingClub.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class AppUser : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
    public bool EmailVerified { get; set; }
}

public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
}

public sealed class ResourceItem : BaseEntity
{
    public string ResourceType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

// Phase 2: Core Club Features

public sealed class Sport : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty;
    public Guid? CoachId { get; set; }
    public Guid? BranchId { get; set; }
}

public sealed class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public Guid? BranchId { get; set; }
}

public sealed class Subscription : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public string LinkedSportIds { get; set; } = "[]"; // JSON array stored as string
}

public sealed class Ticket : BaseEntity
{
    public string EventName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime AvailableSinceUtc { get; set; }
    public DateTime AvailableUntilUtc { get; set; }
    public Guid CreatedBy { get; set; }
}

public sealed class TicketBooking : BaseEntity
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public int QuantityBooked { get; set; }
    public DateTime BookedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class Reservation : BaseEntity
{
    public Guid MemberId { get; set; }
    public Guid SportId { get; set; }
    public DateTime ReservationDateUtc { get; set; }
    public string TimeSlot { get; set; } = string.Empty; // Format: "09:00-10:00"
    public string Status { get; set; } = "Confirmed"; // Confirmed, Cancelled
    public string? Notes { get; set; }
}

public sealed class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? BranchId { get; set; }
    public Guid CreatedBy { get; set; }
}

public sealed class Offer : BaseEntity
{
    public Guid StoreId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
}
