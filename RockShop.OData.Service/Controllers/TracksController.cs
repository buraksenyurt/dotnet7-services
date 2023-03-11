using Microsoft.AspNetCore.Mvc; // for IActionResult
using Microsoft.AspNetCore.OData.Query; // For enabling OData queries
using Microsoft.AspNetCore.OData.Routing.Controllers; //// For routing
using RockShop.Shared;

namespace RockShop.OData.Service.Controllers;

public class TracksController
    : ODataController
{
    protected readonly ChinookDbContext _context;

    public TracksController(ChinookDbContext context)
    {
        _context = context;
    }

    [EnableQuery]
    public IActionResult Get(string version = "1")
    {
        return Ok(_context.Tracks);
    }

    [EnableQuery]
    public IActionResult Get(int key, string version = "1")
    {
        var tracks = _context.Tracks.Where(a => a.TrackId == key);
        Track? track = tracks.FirstOrDefault();
        if (tracks == null || track == null)
        {
            return NotFound($"Track {key} not found");
        }

        if (version == "2")
        {
            var filtered = tracks.Select(t => new
            {
                t.Name,
                t.Composer,
                t.AlbumId,
                t.MediaTypeId
            });
            return Ok(filtered);
        }

        return Ok(track);
    }
}