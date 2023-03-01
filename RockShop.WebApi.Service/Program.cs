using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using RockShop.Shared;
using RockShop.WebApi.Service.Dtos.Response;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddChinookDbContext();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/ping", () => "Pong").ExcludeFromDescription();

int pageSize = 5;
app.MapGet("api/albums", (
    [FromServices] ChinookDbContext db,
    [FromQuery] int? page) =>
        db.Albums.Skip(((page ?? 1) - 1) * pageSize).Take(pageSize)).WithName("GetAlbums")
    .WithOpenApi(operation =>
    {
        operation.Description = "Get albums with paging";
        return operation;
    })
    .Produces<Album[]>(StatusCodes.Status200OK);

app.MapGet("api/artists", (
    [FromServices] ChinookDbContext db,
    [FromQuery] int? page) =>
        db.Artists.Skip(((page ?? 1) - 1) * pageSize).Take(pageSize)).WithName("GetArtists")
    .WithOpenApi(operation =>
    {
        operation.Description = "Get artists with paging";
        return operation;
    })
    .Produces<Artist[]>(StatusCodes.Status200OK);

app.MapGet("api/tracks", (
    [FromServices] ChinookDbContext db,
    [FromQuery] int? page) =>
        {
            var result = from t in db.Tracks
                         join a in db.Albums on t.AlbumId equals a.AlbumId
                         join g in db.Genres on t.GenreId equals g.GenreId
                         join m in db.MediaTypes on t.MediaTypeId equals m.MediaTypeId
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
            return result.Skip(((page ?? 1) - 1) * pageSize).Take(pageSize);
        })
    .WithName("GetTracks")
    .WithOpenApi(operation =>
    {
        operation.Description = "All tracks with paging";
        return operation;
    })
    .Produces<TrackDto>(StatusCodes.Status200OK);


app.MapGet("api/albums/{id:int}", async Task<Results<Ok<Album>, NotFound>> (
    [FromServices] ChinookDbContext db,
    [FromRoute] int? id) =>
        await db.Albums.FindAsync(id) is Album album ? TypedResults.Ok(album) : TypedResults.NotFound()
    )
    .WithName("GetAlbumById")
    .WithOpenApi(operation =>
    {
        operation.Description = "Get album by id";
        return operation;
    })
    .Produces<Album>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);


app.MapGet("api/customers/{country}", (
    [FromServices] ChinookDbContext db,
    [FromRoute] string country) =>
        db.Customers
            .Where(c => c.Country == country)
            .Select(c => new CustomerDto
            {
                CustomerId = c.CustomerId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                City = c.City,
                Email = c.Email
            }
            )
            .OrderBy(c => c.LastName))
    .WithName("GetCustomersByCountry")
    .WithOpenApi(operation =>
    {
        operation.Description = "Get customers by Country";
        return operation;
    })
    .Produces<CustomerDto>(StatusCodes.Status200OK);

app.MapGet("api/invoices/totalsales/top/{count}", (
    [FromServices] ChinookDbContext db,
    [FromRoute] int count) =>
        {
            if (count <= 0 || count > 10)
                return null;

            return (from i in db.Invoices
                    group i by i.BillingCountry into g
                    select new TotalSalesByCountryDto
                    {
                        Country = g.Key,
                        Total = g.Sum(x => x.Total)
                    }).OrderByDescending(x => x.Total)
            .Take(count);
        }
    )
    .WithName("TotalSalesByCountry")
    .WithOpenApi(operation =>
    {
        operation.Description = "Total Sales By Country (with count)";
        return operation;
    })
    .Produces<TotalSalesByCountryDto>(StatusCodes.Status200OK);

app.MapGet("api/albums/{artistName}", (
    [FromServices] ChinookDbContext db,
    [FromRoute] string artistName) =>
    {
        if (string.IsNullOrEmpty(artistName))
            return null;

        return (from a in db.Albums
                join ar in db.Artists
                on a.ArtistId equals ar.ArtistId
                where !string.IsNullOrEmpty(ar.Name) && ar.Name.ToUpper().StartsWith(artistName.ToUpper())
                select a).OrderBy(a => a.Title);
    }
    )
    .WithName("GetAlbumsByArtistName")
    .WithOpenApi(operation =>
    {
        operation.Description = "Get albums by artist name";
        return operation;
    })
    .Produces<List<Album>>(StatusCodes.Status200OK);

app.MapGet("api/artists/{id:int}", async Task<Results<Ok<Artist>, NotFound>> (
    [FromServices] ChinookDbContext db,
    [FromRoute] int? id) =>
        await db.Artists.FindAsync(id) is Artist artist ? TypedResults.Ok(artist) : TypedResults.NotFound()
    )
    .WithName("GetArtistById")
    .WithOpenApi(operation =>
    {
        operation.Description = "Get artist by id";
        return operation;
    })
    .Produces<Artist>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapDelete("api/albums/{id:int}", async (
    [FromServices] ChinookDbContext db,
    [FromRoute] int? id) =>
    {
        if (await db.Albums.FindAsync(id) is Album album)
        {
            db.Albums.Remove(album);
            await db.SaveChangesAsync();
            return Results.NoContent();

        }
        return Results.NotFound();
    })
    .WithName("DeleteAlbumById")
    .WithOpenApi(operation =>
    {
        operation.Description = "Delete album by id";
        return operation;
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("api/artists", async ([FromServices] ChinookDbContext db, [FromBody] PostArtistDto artist) =>
{
    var newArtist = new Artist
    {
        Name = artist.Name
    };
    var createdArtist = await db.Artists.AddAsync(newArtist);
    await db.SaveChangesAsync();

    await db.Albums.AddRangeAsync(artist.Albums.Select(a => new Album
    {
        Title = a.Title ?? "None",
        ArtistId = createdArtist.Entity.ArtistId
    }).ToArray());
    await db.SaveChangesAsync();

    return Results.Created($"api/artists/{createdArtist.Entity.ArtistId}", newArtist);
})
.WithName("CreateArtist")
.WithOpenApi(operation =>
    {
        operation.Description = "Create a new Artist";
        return operation;
    })
.Produces<PostArtistDto>(StatusCodes.Status201Created);

app.MapPut("api/artists/{id:int}", async (
    [FromRoute] int id,
    [FromBody] PutArtistDto artistDto,
    [FromServices] ChinookDbContext db) =>
{
    Artist? founded = await db.Artists.FindAsync(id);
    if (founded is null)
        return Results.NotFound();

    founded.Name = artistDto.Name;
    await db.SaveChangesAsync();

    return Results.NoContent();
}
)
.WithName("UpdateArtistName")
.WithOpenApi(operation =>
    {
        operation.Description = "Update artist name by id";
        return operation;
    })
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status204NoContent);


app.Run();