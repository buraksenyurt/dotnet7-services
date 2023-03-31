using Microsoft.AspNetCore.SignalR.Client;
using RockShop.Common.Models.SignalR;

Write("Enter server name to trace: ");
string? serverName = ReadLine();

if (string.IsNullOrEmpty(serverName))
{
    serverName = "AZNLPRD07";
}

HubConnection conn = new HubConnectionBuilder()
        .WithUrl("https://localhost:7044/techReport",
                    (opts) =>
                    {
                        opts.HttpMessageHandlerFactory = (message) =>
                        {
                            if (message is HttpClientHandler clientHandler)
                                // to bypass SSL certificate(use for development only)
                                clientHandler.ServerCertificateCustomValidationCallback +=
                                    (sender, certificate, chain, sslPolicyErrors) => { return true; };
                            return message;
                        };
                    })
        .Build();

await conn.StartAsync();
WriteLine($"Starting hub connection for '{serverName}'");

try
{
    CancellationTokenSource cancellationTokenSource = new();
    IAsyncEnumerable<Health> report = conn.StreamAsync<Health>("GetHealthReports", serverName, cancellationTokenSource.Token);

    await foreach (Health health in report)
    {
        WriteLine($"{health.ServerName} CPU %{health.CpuUsageRate} Memory %{health.MemoryRate}");
        Write("Continue? (y/n) ");
        ConsoleKey key = ReadKey().Key;
        if (key == ConsoleKey.N)
        {
            cancellationTokenSource.Cancel();
        }
        WriteLine();
    }
}
catch (Exception excp)
{
    WriteLine(excp.Message);
}
WriteLine("\nStreaming completed");

await conn.SendAsync("UploadServerNamesAsync", LoadRequestedServerNames());
WriteLine("Uploading requested server names.\nPress 'ENTER' to exit");
ReadLine();
WriteLine("Shutting down...");