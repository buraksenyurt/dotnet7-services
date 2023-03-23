using RockShop.Grpc.Service.Json.Services;
using RockShop.Shared;

var builder = WebApplication.CreateBuilder(args);

// JSON supoort for GRPC
builder.Services.AddGrpc().AddJsonTranscoding();

// EF Support
builder.Services.AddChinookDbContext();

var app = builder.Build();

app.MapGrpcService<JukeBoxService>();

app.Run();
