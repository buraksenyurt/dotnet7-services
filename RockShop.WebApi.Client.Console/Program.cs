using RockShop.Shared;
using System.Net.Http.Json;

// Get Client name from user
WriteLine("Web API Client Tester");
WriteLine("Enter client name");
string? clientName = ReadLine();

// If not client name we can use random GUID value to identify client.
if (string.IsNullOrEmpty(clientName))
{
    clientName = $"terminal-client-{Guid.NewGuid()}";
}
WriteLine($"Your client Id is {clientName}");

// Crate HTTP Client object instance
HttpClient client = new();
// Set root address
client.BaseAddress = new("http://localhost:5221");
// Clear standart HTTP Accept header and add a new one (json)
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new("application/json"));
// Add new Request Header.
// Service side rate policy use this identity
client.DefaultRequestHeaders.Add("Client-Identity", clientName);

// infinite loop to test rate limit behaviors
while (true)
{
    Write("{0:hh:mm:ss}", DateTime.UtcNow);
    int waitTime = 1;
    try
    {
        // A sample HTTP Get request for fetching albums
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
            // We will probably get HTTP 429 status code due to rate limit rules. 
            // Of course, when the set limits are exceeded.
            WriteLine($"{(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

            // Get retry after values from Response header(For informational purposes only)
            var retryTime = response.Headers.GetValues("Retry-After").ToArray()[0];
            if (Int32.TryParse(retryTime, out waitTime))
            {
                WriteLine($"Retry after {waitTime} seconds");
            }
        }
    }
    catch (Exception e)
    {
        WriteLine(e.Message);
    }

    // Wait for secodns just to simulate the DDOS
    await Task.Delay(TimeSpan.FromSeconds(waitTime));
}