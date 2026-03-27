using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SportingClub.Domain;

namespace SportingClub.Infrastructure;

public sealed class SportingClubDbContext : DbContext
{
    public SportingClubDbContext(DbContextOptions<SportingClubDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ResourceItem> ResourceItems => Set<ResourceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(b =>
        {
            b.ToTable("app_users");
            b.HasKey(x => x.Id);
            b.Property(x => x.Email).IsRequired();
            b.Property(x => x.FullName).IsRequired();
            b.Property(x => x.PasswordHash).IsRequired();
            b.Property(x => x.Role).IsRequired();
            b.Property(x => x.EmailVerified).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("refresh_tokens");
            b.HasKey(x => x.Id);
            b.Property(x => x.Token).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.ExpiresAtUtc).IsRequired();
            b.Property(x => x.IsRevoked).IsRequired();
            b.HasIndex(x => x.Token).IsUnique();
        });

        modelBuilder.Entity<ResourceItem>(b =>
        {
            b.ToTable("resource_items");
            b.HasKey(x => x.Id);
            b.Property(x => x.ResourceType).IsRequired();
            b.Property(x => x.Name).IsRequired();

            // Store Attributes as JSON text/jsonb so the API can keep its flexible metadata model.
            var dictToJson = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ??
                     new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

            b.Property(x => x.Attributes)
                .HasConversion(dictToJson)
                .HasColumnType("jsonb");

            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.Property(x => x.UpdatedAtUtc).IsRequired();
            b.HasIndex(x => new { x.ResourceType, x.Name });
        });
    }
}

