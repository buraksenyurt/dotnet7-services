using RockShop.Shared;
using System.Net.Http.Json;

WriteLine("Web API Client Tester");
WriteLine("Enter client name");
string? clientName = ReadLine();

if (string.IsNullOrEmpty(clientName))
{
    clientName = $"terminal-client-{Guid.NewGuid()}";
}

WriteLine($"Your client Id is {clientName}");
HttpClient client = new();

client.BaseAddress = new("http://localhost:5221");
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new("application/json"));
client.DefaultRequestHeaders.Add("Client-Identity", clientName);

while (true)
{
    Write("{0:hh:mm:ss}", DateTime.UtcNow);

    try
    {
        HttpResponseMessage response = await client.GetAsync("api/albums");
        if (response.IsSuccessStatusCode)
        {
            Album[]? albums = await response.Content.ReadFromJsonAsync<Album[]>();
            if (albums != null)
            {
                foreach (var album in albums)
                {
                    WriteLine(album.Title);
                }
            }
        }
        else
        {
            WriteLine($"{(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        }
    }
    catch (Exception e)
    {
        WriteLine(e.Message);
    }

    await Task.Delay(TimeSpan.FromSeconds(1));
}