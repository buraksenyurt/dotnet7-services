using StrawberryShake;
using Microsoft.Extensions.DependencyInjection;
using RockShop.GraphQL.Client.Console;

var serviceCollection = new ServiceCollection();
serviceCollection.AddRockShopClient().ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost:5034/graphql"));
IServiceProvider services = serviceCollection.BuildServiceProvider();
IRockShopClient client = services.GetRequiredService<IRockShopClient>();

var result = await client.GetTracks.ExecuteAsync();
result.EnsureNoErrors();

if (result.Data != null)
{
    foreach (var trk in result.Data.Tracks)
    {
        Console.WriteLine($"{trk.Album},{trk.Name},{trk.Composer}");
    }
}
