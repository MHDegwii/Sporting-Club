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
