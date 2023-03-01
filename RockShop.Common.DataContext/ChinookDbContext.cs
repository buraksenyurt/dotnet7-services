using Microsoft.EntityFrameworkCore;

namespace RockShop.Shared;

public partial class ChinookDbContext : DbContext
{
    private static readonly DataRefreshedInterceptor dataRefreshedInterceptor = new();
    public ChinookDbContext()
    {
    }

    public ChinookDbContext(DbContextOptions<ChinookDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceLine> InvoiceLines { get; set; }

    public virtual DbSet<MediaType> MediaTypes { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=chinook;Username=scoth;Password=tiger");
        }
        optionsBuilder.AddInterceptors(dataRefreshedInterceptor);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("Album_pkey");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("Artist_pkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("Customer_pkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("Employee_pkey");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("Genre_pkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("Invoice_pkey");
        });

        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.HasKey(e => e.InvoiceLineId).HasName("InvoiceLine_pkey");
        });

        modelBuilder.Entity<MediaType>(entity =>
        {
            entity.HasKey(e => e.MediaTypeId).HasName("MediaType_pkey");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("Playlist_pkey");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(e => e.TrackId).HasName("Track_pkey");
        });

        modelBuilder.UseSerialColumns();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
