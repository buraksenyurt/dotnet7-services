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
    public IActionResult Get()
    {
        return Ok(_context.Tracks);
    }

    [EnableQuery]
    public IActionResult Get(int key)
    {
        return Ok(_context.Tracks.Where(a => a.TrackId == key));
    }
}