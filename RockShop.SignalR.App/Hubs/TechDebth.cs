using Microsoft.AspNetCore.SignalR;
using RockShop.Common.Models.SignalR;
using System.Runtime.CompilerServices;

namespace RockShop.SignalR.App.Hubs;

public class TechDebth
    : Hub
{
    private readonly ILogger<TechDebth> _logger;
    public TechDebth(ILogger<TechDebth> logger)
    {
        _logger = logger;
    }
    public async IAsyncEnumerable<Health> GetHealthReports(string serverName, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetHealthReports called");
        var cpuV = 50.50;
        var memV = 36.50;
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            cpuV += (Random.Shared.NextDouble() * 10.0) - 3.14;
            memV += (Random.Shared.NextDouble() * 10.0) - 5.0;
            Health measure = new Health
            {
                ServerName = serverName,
                MemoryRate = memV,
                CpuUsageRate = cpuV
            };
            _logger.LogInformation(measure.ToString());
            yield return measure;
            await Task.Delay(Random.Shared.Next(1, 10) * 1000, cancellationToken);
        }
    }

    public async Task UploadServerNamesAsync(IAsyncEnumerable<string> serverNames)
    {
        await foreach (string server in serverNames)
        {
            _logger.LogWarning($"Loading health values of {server}");
            // Do Something with incoming servers
        }
    }
}