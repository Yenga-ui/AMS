using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly AssetContext _context;

    public LocationController(AssetContext context)
    {
        _context = context;
    }

    // GET: api/Location/locations
    [HttpGet("locations")]
    public async Task<ActionResult<IEnumerable<object>>> GetLocations()
    {
        var locations = await _context.DtLocations
            .Select(l => new
            {
                l.Id,
                l.Location,
                l.CreatedDate,
                l.LastUpdated
            })
            .ToListAsync();

        return Ok(locations);
    }

    // GET: api/Location/locations/5
    [HttpGet("locations/{id}")]
    public async Task<ActionResult<object>> GetLocation(int id)
    {
        var location = await _context.DtLocations
            .Where(l => l.Id == id)
            .Select(l => new
            {
                l.Id,
                l.Location,
                l.CreatedDate,
                l.LastUpdated
            })
            .FirstOrDefaultAsync();

        if (location == null)
        {
            return NotFound();
        }

        return location;
    }

    // POST: api/Location/locations
    [HttpPost("locations")]
    public async Task<ActionResult<object>> CreateLocation(LocationCreateDto locationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if location name already exists
        if (await _context.DtLocations.AnyAsync(l => l.Location == locationDto.Location))
        {
            return BadRequest("Location name already exists");
        }

        var location = new DtLocation
        {
            Location = locationDto.Location,
            CreatedDate = DateTime.Now
        };

        _context.DtLocations.Add(location);
        await _context.SaveChangesAsync();

        var createdLocation = new
        {
            location.Id,
            location.Location,
            location.CreatedDate
        };

        return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, createdLocation);
    }

    // PUT: api/Location/locations/5
    [HttpPut("locations/{id}")]
    public async Task<IActionResult> UpdateLocation(int id, LocationUpdateDto locationDto)
    {
        if (id != locationDto.Id)
        {
            return BadRequest();
        }

        var location = await _context.DtLocations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        // Check if location name already exists (excluding current location)
        if (await _context.DtLocations.AnyAsync(l => l.Location == locationDto.Location && l.Id != id))
        {
            return BadRequest("Location name already exists");
        }

        location.Location = locationDto.Location;
        location.LastUpdated = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LocationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Location/locations/5
    [HttpDelete("locations/{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await _context.DtLocations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        _context.DtLocations.Remove(location);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LocationExists(int id)
    {
        return _context.DtLocations.Any(e => e.Id == id);
    }
}

// DTOs for location operations
public class LocationCreateDto
{
    public string Location { get; set; }
}

public class LocationUpdateDto
{
    public int Id { get; set; }
    public string Location { get; set; }
}