using Grpc.Core;
using RockShop.Shared;

namespace RockShop.Grpc.Service.Json.Services;

public class JukeBoxService
    : JukeBox.JukeBoxBase
{
    private readonly ILogger<JukeBoxService> _logger;
    private readonly ChinookDbContext _dbContext;
    public JukeBoxService(ILogger<JukeBoxService> logger, ChinookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public override Task<AlbumReply> GetAlbums(AlbumRequest request, ServerCallContext context)
    {
        var albums = _dbContext
            .Albums
            .OrderBy(a => a.AlbumId)
            .Skip((request.PageNumber - 1) * 10)
            .Take(10)
            .Select(a => new AlbumMessage
            {
                AlbumId = a.AlbumId,
                Title = a.Title
            });

        AlbumReply reply = new();
        reply.Data.AddRange(albums);
        return Task.FromResult(reply);
    }
}
