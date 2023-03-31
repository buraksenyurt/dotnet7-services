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
    public async IAsyncEnumerable<Measure> GetTechReportsUpdatesAsync(string measureName, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var randomValue = Random.Shared.NextDouble();
            Measure measure = new Measure
            {
                Name = measureName,
                Rate = randomValue
            };
            _logger.LogInformation($"{measure.Name}, {measure.Rate}");
            yield return measure;
            await Task.Delay(Random.Shared.Next(1, 10) * 1000, cancellationToken);
        }
    }

    public async Task UploadMeasurementsAsync(IAsyncEnumerable<string> measures)
    {
        await foreach (string m in measures)
        {
            _logger.LogWarning($"Loading {m}");
        }
    }
}