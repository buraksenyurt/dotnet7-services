using RockShop.Grpc.Service.Services;
using RockShop.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

// Server Reflection support
builder.Services.AddGrpcReflection();

// EF
builder.Services.AddChinookDbContext();

var app = builder.Build();

app.MapGrpcService<JukeBoxService>();

// Serve Reflection support
IWebHostEnvironment env = app.Environment;
if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
