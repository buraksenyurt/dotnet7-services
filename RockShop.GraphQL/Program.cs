using RockShop.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// GraphQL support
builder.Services.AddGraphQLServer().AddQueryType<Query>();
var app = builder.Build();

app.MapGet("/", () => "For GraphQL server -> http://localhost:5034/graphql");

app.MapGraphQL();
app.Run();
