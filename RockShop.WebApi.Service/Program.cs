using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using RockShop.Shared;

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

app.MapGet("/Ping", () => "Pong");

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
    .Produces<Album[]>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("api/customers/{country}", (
    [FromServices] ChinookDbContext db,
    [FromRoute] string country) =>
        db.Customers
            .Where(c => c.Country == country)
            .Select(c => new { c.CustomerId, c.FirstName, c.LastName, c.City, c.Email })
            .OrderBy(c => c.LastName))
    .WithName("GetCustomersByCountry")
    .WithOpenApi(operation =>
    {
        operation.Description = "Get customers by Country";
        return operation;
    })
    .Produces<Customer>(StatusCodes.Status200OK);

app.MapGet("api/invoices/totalsales/top/five", (
    [FromServices] ChinookDbContext db) =>
        (from i in db.Invoices
         group i by i.BillingCountry into g
         select new
         {
             BillingCountry = g.Key,
             TotalSales = g.Sum(x => x.Total)
         }).OrderByDescending(x => x.TotalSales)
         .Take(5)
    )
    .WithName("TotalSalesByCountry")
    .WithOpenApi(operation =>
    {
        operation.Description = "Total Sales By Country(Top Five)";
        return operation;
    })
    .Produces<Invoice>(StatusCodes.Status200OK);


app.Run();