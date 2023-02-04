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
            optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=scoth;Password=tiger");
        }
        optionsBuilder.AddInterceptors(dataRefreshedInterceptor);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.Property(e => e.AlbumId).ValueGeneratedNever();

            entity.HasOne(d => d.Artist).WithMany(p => p.Albums)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlbumArtistId");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.Property(e => e.ArtistId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).ValueGeneratedNever();

            entity.HasOne(d => d.SupportRep).WithMany(p => p.Customers).HasConstraintName("FK_CustomerSupportRepId");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.EmployeeId).ValueGeneratedNever();

            entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation).HasConstraintName("FK_EmployeeReportsTo");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(e => e.GenreId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(e => e.InvoiceId).ValueGeneratedNever();

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceCustomerId");
        });

        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.Property(e => e.InvoiceLineId).ValueGeneratedNever();

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceLines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceLineInvoiceId");

            entity.HasOne(d => d.Track).WithMany(p => p.InvoiceLines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceLineTrackId");
        });

        modelBuilder.Entity<MediaType>(entity =>
        {
            entity.Property(e => e.MediaTypeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.Property(e => e.PlaylistId).ValueGeneratedNever();

            entity.HasMany(d => d.Tracks).WithMany(p => p.Playlists)
                .UsingEntity<Dictionary<string, object>>(
                    "PlaylistTrack",
                    r => r.HasOne<Track>().WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PlaylistTrackTrackId"),
                    l => l.HasOne<Playlist>().WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PlaylistTrackPlaylistId"),
                    j =>
                    {
                        j.HasKey("PlaylistId", "TrackId");
                        j.ToTable("PlaylistTrack");
                        j.HasIndex(new[] { "TrackId" }, "IFK_PlaylistTrackTrackId");
                    });
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.Property(e => e.TrackId).ValueGeneratedNever();

            entity.HasOne(d => d.Album).WithMany(p => p.Tracks).HasConstraintName("FK_TrackAlbumId");

            entity.HasOne(d => d.Genre).WithMany(p => p.Tracks).HasConstraintName("FK_TrackGenreId");

            entity.HasOne(d => d.MediaType).WithMany(p => p.Tracks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TrackMediaTypeId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
