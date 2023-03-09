using Microsoft.AspNetCore.OData;
using RockShop.Shared;

var builder = WebApplication.CreateBuilder(args);

// We use EF Data Context
builder.Services.AddChinookDbContext();
// Setup for OData.
builder.Services
    .AddControllers()
    .AddOData(options => options
                        .AddRouteComponents(routePrefix: "music", model: GetEdmModelForMusics())
                        .Select()
                        .Expand()
                        .Filter()
                        .OrderBy()
                        .SetMaxTop(10)
                        .Count()
            );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
