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

    // Sample Post operation. In practical OData services does not contain Create,Update, Delete operations.
    // But there is a support for this.
    public IActionResult Post([FromBody] Artist artist)
    {
        _context.Artists.Add(artist);
        _context.SaveChanges();
        return Created(artist);
    }

    public IActionResult Delete(int key)
    {
        var artist = _context.Artists.Where(a => a.ArtistId == key).FirstOrDefault();

        if (artist == null)
            return NotFound("{key} not found");

        _context.Artists.Remove(artist);
        _context.SaveChanges();

        return NoContent();
    }
}