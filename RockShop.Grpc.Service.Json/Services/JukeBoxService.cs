using Grpc.Core;
using RockShop.Shared;

namespace RockShop.Grpc.Service.Json.Services;

/// <summary>
/// JukeBox grpcService
/// </summary>
public class JukeBoxService
    : JukeBox.JukeBoxBase
{
    private readonly ILogger<JukeBoxService> _logger;
    private readonly ChinookDbContext _dbContext;
    /// <summary>
    /// Constructor
    /// </summary>
    public JukeBoxService(ILogger<JukeBoxService> logger, ChinookDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Get albums with paging
    /// </summary>
    /// <param name="request">Includes page number</param>
    /// <param name="context">Server call context</param>
    /// <returns>Album names</returns>
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
