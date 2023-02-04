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
    [FromQuery] int? page) => db.Albums.Skip(((page ?? 1) - 1) * pageSize).Take(pageSize)).WithName("GetAlbums")
    .WithOpenApi(operation =>
    {
        operation.Description = "Get albums with paging";
        return operation;
    })
    .Produces<Album[]>(StatusCodes.Status200OK);

app.Run();