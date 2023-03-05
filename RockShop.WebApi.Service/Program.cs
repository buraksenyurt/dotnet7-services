using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RockShop.Shared;
using RockShop.WebApi.Service.Dtos.Response;
using Microsoft.AspNetCore.HttpLogging;
using AspNetCoreRateLimit;

// Net 7.0 Rate Limiting(PreRelease)
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

// To use 3rd Party Rate Limitings set to false 
//otherwise for using AspNet core internal rate limiting middleware set to true
bool UseMicrosoftRateLimits = true;

var builder = WebApplication.CreateBuilder(args);

// Rate Limit Service middleware
// Using rate limits from appSettings for Clients
if (!UseMicrosoftRateLimits)
{
    // Rate limit counters and rules reside in memory
    builder.Services.AddMemoryCache();
    builder.Services.AddInMemoryRateLimiting();

    // Read rate limit options from appSettings section
    builder.Services.Configure<ClientRateLimitOptions>(
        builder.Configuration.GetSection("ClientRateLimits")
    );
    // Read rate limit policies from appSettings section
    builder.Services.Configure<ClientRateLimitPolicies>(
        builder.Configuration.GetSection("ClientRateLimitsPolicies")
    );
    // Register the configuration to DI Services in a Singleton lifetime mode
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
}


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddChinookDbContext();
builder.Services.AddHttpLogging(options =>
{
    options.RequestHeaders.Add("Origin");
    // Rate limiting headers
    options.RequestHeaders.Add("Client-Identity");
    options.RequestHeaders.Add("Retry-After");

    options.LoggingFields = HttpLoggingFields.All;
});
// With following CORS implementation the index.cshtml page on Mvc Client works without browser CORS error.
var pnMvcClient = "RockShopMvcClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: pnMvcClient
        , policy =>
        {
            policy.WithOrigins("http://localhost:5233");
        });
});

var app = builder.Build();

// Load rate limit policies from configuration by using Seed function
if (!UseMicrosoftRateLimits) // if 3rd party rate limiter is active
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        IClientPolicyStore cps = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
        await cps.SeedAsync();
    }
}
else
{
    // Using Microsoft Rate Limiter
    RateLimiterOptions rateLimiterOptions = new();
    // Look at where we use "only1ReqPer10Seconds" alias in this code
    rateLimiterOptions.AddFixedWindowLimiter(
        policyName: "only1ReqPer10Seconds", options =>
        {
            options.PermitLimit = 1; // Only one request
            options.QueueLimit = 2; // Max queue limit
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Oldest request processing first
            options.Window = TimeSpan.FromSeconds(10); // Rate limit durations in seconds
        }
    );
    // Add middleware to this application
    app.UseRateLimiter(rateLimiterOptions);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();

// Use rate limit
if (!UseMicrosoftRateLimits)
{
    app.UseClientRateLimiting();
}

// app.UseCors(policyName: pnMvcClient); // Enabled the same CORS policy for the whole api service.
app.UseCors(); // CORS Policy added but not active

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
    .Produces<Album[]>(StatusCodes.Status200OK)
    .RequireRateLimiting("only1ReqPer10Seconds"); // We use Microsoft's rate limiting rules which defined by "only1ReqPer10Seconds" alias for this request.

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
    .Produces(StatusCodes.Status404NotFound)
    .RequireCors(policyName: pnMvcClient); // Enabled CORS policy for this endpoint


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
    .Produces<List<Album>>(StatusCodes.Status200OK)
    .RequireCors(policyName: pnMvcClient); // Enabled CORS policy for this endpoint

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