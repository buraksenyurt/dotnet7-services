using RockShop.GraphQL;
using RockShop.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddChinookDbContext();

// GraphQL support
builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<ChinookDbContext>() // to use Entity Framework Context with GraphQL
    .AddQueryType<Query>();
var app = builder.Build();

app.MapGet("/", () => "For GraphQL server -> http://localhost:5034/graphql");

app.MapGraphQL();
app.Run();
