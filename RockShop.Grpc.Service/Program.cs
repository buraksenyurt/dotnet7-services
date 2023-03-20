using RockShop.Grpc.Service.Services;
using RockShop.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddChinookDbContext();

var app = builder.Build();

app.MapGrpcService<JukeBoxService>();

app.Run();
