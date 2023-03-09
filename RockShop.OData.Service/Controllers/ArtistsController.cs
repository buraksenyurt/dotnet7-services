using Microsoft.AspNetCore.Mvc; // for IActionResult
using Microsoft.AspNetCore.OData.Query; // For enabling OData queries
using Microsoft.AspNetCore.OData.Routing.Controllers; //// For routing
using RockShop.Shared;

namespace RockShop.OData.Service.Controllers;

public class ArtistsController
    : ODataController
{
    protected readonly ChinookDbContext _context;

    public ArtistsController(ChinookDbContext context)
    {
        _context = context;
    }

    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Artists);
    }

    [EnableQuery]
    public IActionResult Get(int key)
    {
        return Ok(_context.Artists.Where(a => a.ArtistId == key));
    }
}