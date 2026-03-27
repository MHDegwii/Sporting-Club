using SportingClub.Domain;

namespace SportingClub.Application;

public sealed record RegisterRequest(string Email, string FullName, string Password, string Role);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshRequest(string RefreshToken);
public sealed record ForgotPasswordRequest(string Email);
public sealed record ResetPasswordRequest(string Email, string NewPassword);
public sealed record VerifyEmailRequest(string Email);

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}

public sealed class QueryOptions
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshAsync(RefreshRequest request);
    Task LogoutAsync(string refreshToken);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task VerifyEmailAsync(VerifyEmailRequest request);
}

public interface IResourceService
{
    Task<ResourceItem> CreateAsync(string resourceType, ResourceItem request);
    Task<PagedResult<ResourceItem>> ListAsync(string resourceType, QueryOptions options);
    Task<ResourceItem?> GetAsync(string resourceType, Guid id);
    Task<ResourceItem?> UpdateAsync(string resourceType, Guid id, ResourceItem request);
    Task<bool> DeleteAsync(string resourceType, Guid id);
}

public interface IFileStorageService
{
    Task<string> UploadAsync(string fileName, string base64Content);
}

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public interface INotificationService
{
    Task BroadcastAsync(string title, string message);
    Task SendToUserAsync(string userEmail, string title, string message);
}

public interface IAnalyticsService
{
    IDictionary<string, object> GetDashboardSummary();
}

// ============= PHASE 2 CONTRACTS & DTOs =============

// Sport DTOs & Service
public sealed record CreateSportRequest(string Name, string Description, string Schedule, Guid? CoachId = null, Guid? BranchId = null);
public sealed record UpdateSportRequest(string Name, string Description, string Schedule, Guid? CoachId = null, Guid? BranchId = null);

public interface ISportService
{
    Task<Sport> CreateAsync(CreateSportRequest request);
    Task<PagedResult<Sport>> ListAsync(QueryOptions options);
    Task<Sport?> GetAsync(Guid id);
    Task<Sport?> UpdateAsync(Guid id, UpdateSportRequest request);
    Task<bool> DeleteAsync(Guid id);
}

// Service DTOs & Service
public sealed record CreateServiceRequest(string Name, string Description, string Status = "Available", Guid? BranchId = null);
public sealed record UpdateServiceRequest(string Name, string Description, string Status, Guid? BranchId = null);

public interface IServiceService
{
    Task<Service> CreateAsync(CreateServiceRequest request);
    Task<PagedResult<Service>> ListAsync(QueryOptions options);
    Task<Service?> GetAsync(Guid id);
    Task<Service?> UpdateAsync(Guid id, UpdateServiceRequest request);
    Task<bool> DeleteAsync(Guid id);
}

// Subscription DTOs & Service
public sealed record CreateSubscriptionRequest(string Name, decimal Price, int DurationDays, string LinkedSportIds = "[]");
public sealed record UpdateSubscriptionRequest(string Name, decimal Price, int DurationDays, string LinkedSportIds = "[]");

public interface ISubscriptionService
{
    Task<Subscription> CreateAsync(CreateSubscriptionRequest request);
    Task<PagedResult<Subscription>> ListAsync(QueryOptions options);
    Task<Subscription?> GetAsync(Guid id);
    Task<Subscription?> UpdateAsync(Guid id, UpdateSubscriptionRequest request);
    Task<bool> DeleteAsync(Guid id);
}

// Ticket DTOs & Service
public sealed record CreateTicketRequest(string EventName, int Quantity, decimal Price, DateTime AvailableSinceUtc, DateTime AvailableUntilUtc);
public sealed record UpdateTicketRequest(string EventName, int Quantity, decimal Price, DateTime AvailableSinceUtc, DateTime AvailableUntilUtc);
public sealed record BookTicketRequest(Guid TicketId, int Quantity);

public interface ITicketService
{
    Task<Ticket> CreateAsync(CreateTicketRequest request, Guid createdBy);
    Task<PagedResult<Ticket>> ListAsync(QueryOptions options);
    Task<Ticket?> GetAsync(Guid id);
    Task<Ticket?> UpdateAsync(Guid id, UpdateTicketRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<TicketBooking> BookTicketAsync(BookTicketRequest request, Guid userId);
    Task<PagedResult<TicketBooking>> ListUserBookingsAsync(Guid userId, QueryOptions options);
}

// Reservation DTOs & Service
public sealed record CreateReservationRequest(Guid MemberId, Guid SportId, DateTime ReservationDateUtc, string TimeSlot, string? Notes = null);
public sealed record UpdateReservationRequest(DateTime ReservationDateUtc, string TimeSlot, string Status, string? Notes = null);

public interface IReservationService
{
    Task<Reservation> CreateAsync(CreateReservationRequest request);
    Task<PagedResult<Reservation>> ListAsync(QueryOptions options);
    Task<Reservation?> GetAsync(Guid id);
    Task<Reservation?> UpdateAsync(Guid id, UpdateReservationRequest request);
    Task<bool> CancelAsync(Guid id);
    Task<IReadOnlyList<string>> GetAvailableSlotsAsync(Guid sportId, DateTime date);
    Task<bool> CheckConflictAsync(Guid sportId, DateTime date, string timeSlot);
}

// Store DTOs & Service
public sealed record CreateStoreRequest(string Name, string Description, Guid? BranchId = null);
public sealed record UpdateStoreRequest(string Name, string Description, Guid? BranchId = null);

public interface IStoreService
{
    Task<Store> CreateAsync(CreateStoreRequest request, Guid createdBy);
    Task<PagedResult<Store>> ListAsync(QueryOptions options);
    Task<Store?> GetAsync(Guid id);
    Task<Store?> UpdateAsync(Guid id, UpdateStoreRequest request);
    Task<bool> DeleteAsync(Guid id);
}

// Offer DTOs & Service
public sealed record CreateOfferRequest(Guid StoreId, string Description, decimal DiscountPercentage, DateTime StartDateUtc, DateTime EndDateUtc);
public sealed record UpdateOfferRequest(string Description, decimal DiscountPercentage, DateTime StartDateUtc, DateTime EndDateUtc);

public interface IOfferService
{
    Task<Offer> CreateAsync(CreateOfferRequest request);
    Task<PagedResult<Offer>> ListAsync(QueryOptions options);
    Task<Offer?> GetAsync(Guid id);
    Task<Offer?> UpdateAsync(Guid id, UpdateOfferRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<PagedResult<Offer>> ListActiveOffersAsync(QueryOptions options);
}
