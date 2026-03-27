using System.Text.Json;
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

    // Phase 2 Entities
    public DbSet<Sport> Sports => Set<Sport>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketBooking> TicketBookings => Set<TicketBooking>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Offer> Offers => Set<Offer>();

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

            // Store Attributes as JSON so the API can keep its flexible metadata model.
            var dictToJson = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ??
                     new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

            b.Property(x => x.Attributes)
                .HasConversion(dictToJson)
                .HasColumnType("text");

            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.Property(x => x.UpdatedAtUtc).IsRequired();
            b.HasIndex(x => new { x.ResourceType, x.Name });
        });

        // Phase 2 Configurations
        modelBuilder.Entity<Sport>(b =>
        {
            b.ToTable("sports");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Description).IsRequired();
            b.Property(x => x.Schedule).IsRequired();
            b.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<Service>(b =>
        {
            b.ToTable("services");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Description).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<Subscription>(b =>
        {
            b.ToTable("subscriptions");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Price).IsRequired();
            b.Property(x => x.DurationDays).IsRequired();
        });

        modelBuilder.Entity<Ticket>(b =>
        {
            b.ToTable("tickets");
            b.HasKey(x => x.Id);
            b.Property(x => x.EventName).IsRequired();
            b.Property(x => x.Quantity).IsRequired();
            b.Property(x => x.Price).IsRequired();
            b.Property(x => x.CreatedBy).IsRequired();
            b.HasIndex(x => x.AvailableSinceUtc);
            b.HasIndex(x => x.AvailableUntilUtc);
        });

        modelBuilder.Entity<TicketBooking>(b =>
        {
            b.ToTable("ticket_bookings");
            b.HasKey(x => x.Id);
            b.Property(x => x.TicketId).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.QuantityBooked).IsRequired();
            b.HasIndex(x => new { x.TicketId, x.UserId }).IsUnique();
        });

        modelBuilder.Entity<Reservation>(b =>
        {
            b.ToTable("reservations");
            b.HasKey(x => x.Id);
            b.Property(x => x.MemberId).IsRequired();
            b.Property(x => x.SportId).IsRequired();
            b.Property(x => x.ReservationDateUtc).IsRequired();
            b.Property(x => x.TimeSlot).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.HasIndex(x => new { x.SportId, x.ReservationDateUtc, x.TimeSlot });
            b.HasIndex(x => x.MemberId);
            b.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<Store>(b =>
        {
            b.ToTable("stores");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Description).IsRequired();
            b.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<Offer>(b =>
        {
            b.ToTable("offers");
            b.HasKey(x => x.Id);
            b.Property(x => x.StoreId).IsRequired();
            b.Property(x => x.Description).IsRequired();
            b.Property(x => x.DiscountPercentage).IsRequired();
            b.HasIndex(x => x.StoreId);
            b.HasIndex(x => x.EndDateUtc);
        });
    }
}

