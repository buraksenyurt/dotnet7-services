using Microsoft.EntityFrameworkCore;
using RockShop.Shared;

namespace RockShop.GraphQL;

public class Query
{
    public string Ping() => "Pong";
    public int LuckyNum()
    {
        Random rnd = new Random();
        return rnd.Next(1, 100);
    }

    public IQueryable<Artist> GetArtists(ChinookDbContext dbContext, int? page) => dbContext.Artists.Skip(((page ?? 1) - 1) * 10).Take(10);

    public Album? GetAlbum(ChinookDbContext dbContext, int albumId)
    {
        Album? album = dbContext.Albums.Find(albumId);
        if (album == null)
            return null;

        return album;
    }

    public Artist? GetArtist(ChinookDbContext dbContext, int artistId)
    {
        Artist? artist = dbContext.Artists.Find(artistId);
        if (artist == null)
            return null;

        return artist;
    }

    public IQueryable<TotalSalesByCountryDto> GetTotalSalesByCountry(ChinookDbContext dbContext, int count)
    {
        if (count <= 0 || count > 10)
            return null;

        return (from i in dbContext.Invoices
                group i by i.BillingCountry into g
                select new TotalSalesByCountryDto
                {
                    Country = g.Key,
                    Total = g.Sum(x => x.Total)
                }).OrderByDescending(x => x.Total)
        .Take(count);
    }

    public IQueryable<TrackDto> GetTracks(ChinookDbContext dbContext, int? page)
    {
        var result = from t in dbContext.Tracks
                     join a in dbContext.Albums on t.AlbumId equals a.AlbumId
                     join g in dbContext.Genres on t.GenreId equals g.GenreId
                     join m in dbContext.MediaTypes on t.MediaTypeId equals m.MediaTypeId
                     select new TrackDto
                     {
                         Name = t.Name,
                         Composer = t.Composer,
                         Duration = t.Milliseconds,
                         UnitPrice = t.UnitPrice,
                         Album = a.Title,
                         Genre = g.Name,
                         MediaType = m.Name
                     };
        return result.Skip(((page ?? 1) - 1) * 10).Take(10);
    }
}

public class TotalSalesByCountryDto
{
    public string? Country { get; set; }
    public decimal Total { get; set; }
}

public class TrackDto
{
    public string? Name { get; set; }
    public string? Album { get; set; }
    public string? Composer { get; set; }
    public double Duration { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Genre { get; set; }
    public string? MediaType { get; set; }
}