using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
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

public sealed class EfCoreAuthService : IAuthService
{
    private readonly SportingClubDbContext _db;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public EfCoreAuthService(SportingClubDbContext db, IEmailService emailService, IConfiguration configuration)
    {
        _db = db;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await _db.AppUsers.AnyAsync(u => u.Email == request.Email);
        if (exists)
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

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        await _emailService.SendAsync(user.Email, "Verify your email", "Use /verify-email endpoint to verify.");
        return await IssueTokensAndPersistRefreshAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.AppUsers.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || user.PasswordHash != Hash(request.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return await IssueTokensAndPersistRefreshAsync(user);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
    {
        var token = await _db.RefreshTokens.SingleOrDefaultAsync(t =>
            t.Token == request.RefreshToken && !t.IsRevoked && t.ExpiresAtUtc > DateTime.UtcNow);

        if (token is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        // Revoke old token, then issue a new access + refresh token pair.
        token.IsRevoked = true;
        token.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var user = await _db.AppUsers.SingleOrDefaultAsync(u => u.Id == token.UserId);
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        return await IssueTokensAndPersistRefreshAsync(user);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var token = await _db.RefreshTokens.SingleOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);
        if (token is not null)
        {
            token.IsRevoked = true;
            token.UpdatedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        // For now we just send a stub email. Later this can generate and store reset tokens.
        await _emailService.SendAsync(request.Email, "Reset password", "Use /reset-password endpoint.");
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _db.AppUsers.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user is not null)
        {
            user.PasswordHash = Hash(request.NewPassword);
            user.UpdatedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task VerifyEmailAsync(VerifyEmailRequest request)
    {
        var user = await _db.AppUsers.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user is not null)
        {
            user.EmailVerified = true;
            user.UpdatedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    private async Task<AuthResponse> IssueTokensAndPersistRefreshAsync(AppUser user)
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
        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        await _db.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshValue,
            ExpiresAtUtc = expiresAt
        };
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

public sealed class EfCoreResourceService : IResourceService
{
    private readonly SportingClubDbContext _db;

    public EfCoreResourceService(SportingClubDbContext db)
    {
        _db = db;
    }

    public async Task<ResourceItem> CreateAsync(string resourceType, ResourceItem request)
    {
        var normalized = NormalizeResource(resourceType);
        request.Id = Guid.NewGuid();
        request.ResourceType = normalized;
        request.CreatedAtUtc = DateTime.UtcNow;
        request.UpdatedAtUtc = DateTime.UtcNow;

        _db.ResourceItems.Add(request);
        await _db.SaveChangesAsync();
        return request;
    }

    public async Task<PagedResult<ResourceItem>> ListAsync(string resourceType, QueryOptions options)
    {
        var normalized = NormalizeResource(resourceType);

        var query = _db.ResourceItems.Where(x => x.ResourceType == normalized);
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.Name.Contains(options.Search, StringComparison.OrdinalIgnoreCase));
        }

        var sortBy = options.SortBy?.Trim().ToLowerInvariant();
        var sortDir = options.SortDirection?.Trim().ToLowerInvariant();
        var isDesc = sortDir == "desc";

        query = sortBy switch
        {
            "name" => isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "createdatutc" or "createdatutc" or "created_at_utc" => isDesc ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderBy(x => x.Name)
        };

        var totalCount = await query.CountAsync();

        var page = options.Page < 1 ? 1 : options.Page;
        var pageSize = options.PageSize is < 1 or > 200 ? 20 : options.PageSize;
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<ResourceItem>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ResourceItem?> GetAsync(string resourceType, Guid id)
    {
        var normalized = NormalizeResource(resourceType);
        return await _db.ResourceItems.SingleOrDefaultAsync(x => x.ResourceType == normalized && x.Id == id);
    }

    public async Task<ResourceItem?> UpdateAsync(string resourceType, Guid id, ResourceItem request)
    {
        var normalized = NormalizeResource(resourceType);
        var existing = await _db.ResourceItems.SingleOrDefaultAsync(x => x.ResourceType == normalized && x.Id == id);
        if (existing is null)
        {
            return null;
        }

        existing.Name = request.Name;
        existing.Attributes = request.Attributes;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(string resourceType, Guid id)
    {
        var normalized = NormalizeResource(resourceType);
        var existing = await _db.ResourceItems.SingleOrDefaultAsync(x => x.ResourceType == normalized && x.Id == id);
        if (existing is null)
        {
            return false;
        }

        _db.ResourceItems.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
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
