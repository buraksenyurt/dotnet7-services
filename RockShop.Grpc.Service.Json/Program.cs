using RockShop.Grpc.Service.Json.Services;
using RockShop.Shared;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// JSON supoort for GRPC
builder.Services.AddGrpc().AddJsonTranscoding();
// Swagger support
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(options =>
                               options.SwaggerDoc("v1", new OpenApiInfo
                               {
                                   Title = "Rock Shop gRPC Services",
                                   Version = "v1"
                               }));

// EF Support
builder.Services.AddChinookDbContext();

var app = builder.Build();

app.MapGrpcService<JukeBoxService>();

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "RockShop gRPC v1"));

app.Run();
