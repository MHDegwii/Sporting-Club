using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SportingClub.Application;
using SportingClub.Domain;

namespace SportingClub.Infrastructure;

public sealed class InMemoryAuthService : IAuthService
{
    private readonly IDictionary<string, AppUser> _users = new Dictionary<string, AppUser>(StringComparer.OrdinalIgnoreCase);
    private readonly IDictionary<string, RefreshToken> _refreshTokens = new Dictionary<string, RefreshToken>(StringComparer.Ordinal);
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public InMemoryAuthService(IEmailService emailService, IConfiguration configuration)
    {
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (_users.ContainsKey(request.Email))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new AppUser
        {
            Email = request.Email,
            FullName = request.FullName,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Member" : request.Role,
            PasswordHash = Hash(request.Password),
            EmailVerified = false
        };

        _users[user.Email] = user;
        await _emailService.SendAsync(user.Email, "Verify your email", "Use /verify-email endpoint to verify.");
        return IssueTokens(user);
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        if (!_users.TryGetValue(request.Email, out var user) || user.PasswordHash != Hash(request.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return Task.FromResult(IssueTokens(user));
    }

    public Task<AuthResponse> RefreshAsync(RefreshRequest request)
    {
        if (!_refreshTokens.TryGetValue(request.RefreshToken, out var token) || token.IsRevoked || token.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var user = _users.Values.FirstOrDefault(u => u.Id == token.UserId) ?? throw new UnauthorizedAccessException("User not found.");
        token.IsRevoked = true;
        return Task.FromResult(IssueTokens(user));
    }

    public Task LogoutAsync(string refreshToken)
    {
        if (_refreshTokens.TryGetValue(refreshToken, out var token))
        {
            token.IsRevoked = true;
        }

        return Task.CompletedTask;
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        await _emailService.SendAsync(request.Email, "Reset password", "Use /reset-password endpoint.");
    }

    public Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (_users.TryGetValue(request.Email, out var user))
        {
            user.PasswordHash = Hash(request.NewPassword);
            user.UpdatedAtUtc = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task VerifyEmailAsync(VerifyEmailRequest request)
    {
        if (_users.TryGetValue(request.Email, out var user))
        {
            user.EmailVerified = true;
            user.UpdatedAtUtc = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    private AuthResponse IssueTokens(AppUser user)
    {
        var key = _configuration["Jwt:Key"] ?? "sporting-club-dev-super-secret-signing-key";
        var issuer = _configuration["Jwt:Issuer"] ?? "SportingClub";
        var audience = _configuration["Jwt:Audience"] ?? "SportingClubClients";
        var expiryMinutes = int.TryParse(_configuration["Jwt:AccessTokenExpiryMinutes"], out var minutes) ? minutes : 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = expiresAt,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        var refreshValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        _refreshTokens[refreshValue] = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshValue,
            ExpiresAtUtc = expiresAt
        };
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

public sealed class InMemoryResourceService : IResourceService
{
    private readonly Dictionary<string, List<ResourceItem>> _store = new(StringComparer.OrdinalIgnoreCase);

    public Task<ResourceItem> CreateAsync(string resourceType, ResourceItem request)
    {
        var normalized = NormalizeResource(resourceType);
        request.Id = Guid.NewGuid();
        request.ResourceType = normalized;
        request.CreatedAtUtc = DateTime.UtcNow;
        request.UpdatedAtUtc = DateTime.UtcNow;

        if (!_store.ContainsKey(normalized))
        {
            _store[normalized] = new List<ResourceItem>();
        }

        _store[normalized].Add(request);
        return Task.FromResult(request);
    }

    public Task<PagedResult<ResourceItem>> ListAsync(string resourceType, QueryOptions options)
    {
        var normalized = NormalizeResource(resourceType);
        if (!_store.TryGetValue(normalized, out var items))
        {
            items = new List<ResourceItem>();
        }

        IEnumerable<ResourceItem> query = items;
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.Name.Contains(options.Search, StringComparison.OrdinalIgnoreCase));
        }

        query = (options.SortBy?.ToLowerInvariant(), options.SortDirection?.ToLowerInvariant()) switch
        {
            ("name", "desc") => query.OrderByDescending(x => x.Name),
            ("createdatutc", "desc") => query.OrderByDescending(x => x.CreatedAtUtc),
            ("createdatutc", _) => query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderBy(x => x.Name)
        };

        var totalCount = query.Count();
        var page = options.Page < 1 ? 1 : options.Page;
        var pageSize = options.PageSize is < 1 or > 200 ? 20 : options.PageSize;
        var pagedItems = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new PagedResult<ResourceItem>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public Task<ResourceItem?> GetAsync(string resourceType, Guid id)
    {
        var normalized = NormalizeResource(resourceType);
        _store.TryGetValue(normalized, out var items);
        return Task.FromResult(items?.FirstOrDefault(i => i.Id == id));
    }

    public Task<ResourceItem?> UpdateAsync(string resourceType, Guid id, ResourceItem request)
    {
        var normalized = NormalizeResource(resourceType);
        _store.TryGetValue(normalized, out var items);
        var existing = items?.FirstOrDefault(i => i.Id == id);
        if (existing is null)
        {
            return Task.FromResult<ResourceItem?>(null);
        }

        existing.Name = request.Name;
        existing.Attributes = request.Attributes;
        existing.UpdatedAtUtc = DateTime.UtcNow;
        return Task.FromResult<ResourceItem?>(existing);
    }

    public Task<bool> DeleteAsync(string resourceType, Guid id)
    {
        var normalized = NormalizeResource(resourceType);
        if (!_store.TryGetValue(normalized, out var items))
        {
            return Task.FromResult(false);
        }

        var removed = items.RemoveAll(i => i.Id == id) > 0;
        return Task.FromResult(removed);
    }

    private static string NormalizeResource(string resourceType)
    {
        var supported = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "employees", "departments", "members", "sports", "services", "subscriptions",
            "tickets", "reservations", "branches", "stores", "offers", "announcements", "faqs"
        };

        if (!supported.Contains(resourceType))
        {
            throw new ArgumentException($"Unsupported resource type '{resourceType}'.");
        }

        return resourceType.ToLowerInvariant();
    }
}

public sealed class LocalFileStorageService : IFileStorageService
{
    public Task<string> UploadAsync(string fileName, string base64Content)
    {
        var safeName = $"{Guid.NewGuid()}-{fileName}";
        var outputDir = Path.Combine(AppContext.BaseDirectory, "uploads");
        Directory.CreateDirectory(outputDir);
        var outputPath = Path.Combine(outputDir, safeName);
        File.WriteAllBytes(outputPath, Convert.FromBase64String(base64Content));
        return Task.FromResult(outputPath);
    }
}

public sealed class ConsoleEmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string body)
    {
        Console.WriteLine($"EMAIL => To: {to} | Subject: {subject} | Body: {body}");
        return Task.CompletedTask;
    }
}

public sealed class FcmNotificationService : INotificationService
{
    public Task BroadcastAsync(string title, string message)
    {
        Console.WriteLine($"FCM Broadcast => {title}: {message}");
        return Task.CompletedTask;
    }

    public Task SendToUserAsync(string userEmail, string title, string message)
    {
        Console.WriteLine($"FCM Target => {userEmail} | {title}: {message}");
        return Task.CompletedTask;
    }
}

public sealed class InMemoryAnalyticsService : IAnalyticsService
{
    public IDictionary<string, object> GetDashboardSummary() =>
        new Dictionary<string, object>
        {
            ["activeMembers"] = 320,
            ["todayReservations"] = 41,
            ["openTickets"] = 9,
            ["monthlyRevenue"] = 15430.75m
        };
}

public sealed class RenewalReminderBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"[JOB] Renewal reminder tick at {DateTime.UtcNow:O}");
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
