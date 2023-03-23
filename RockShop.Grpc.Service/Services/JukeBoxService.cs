using Grpc.Core;
using RockShop.Shared;

namespace RockShop.Grpc.Service.Services;

public class JukeBoxService : JukeBox.JukeBoxBase
{
    private readonly ILogger<JukeBoxService> _logger;
    private readonly ChinookDbContext _dbContext;

    public JukeBoxService(ILogger<JukeBoxService> logger, ChinookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public override Task<ArtistReply> GetArtists(ArtistRequest request, ServerCallContext context)
    {
        // Testing deadline for higher reliability
        _logger.LogWarning($"Deadline of {context.Deadline:T}. Now {DateTime.UtcNow:T}");
        //await Task.Delay(TimeSpan.FromSeconds(5));

        var artists = _dbContext
            .Artists
            .OrderBy(a => a.ArtistId)
            .Skip((request.PageNumber - 1) * 10)
            .Take(10)
            .Select(a => new ArtistMessage
            {
                ArtistId = a.ArtistId,
                Name = a.Name
            });
        ArtistReply reply = new ArtistReply();
        reply.Data.AddRange(artists);
        return Task.FromResult(reply);
    }

}