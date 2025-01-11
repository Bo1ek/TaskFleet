using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Data;
using TaskFleet.DTOs.Requests;
using TaskFleet.Models;

namespace TaskFleet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LocationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
    {
        return await _context.Locations.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Location>> GetLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
            return NotFound();
        return location;
    }

    [HttpPost]
    public async Task<ActionResult<Location>> CreateLocation(CreateLocationRequest request)
    {
        var location = new Location
        {
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLocation), new { id = location.LocationId }, location);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(int id, Location location)
    {
        if (location.LocationId != id)
            return BadRequest();

        var existingLocation = await _context.Locations.FindAsync(id);
        if (existingLocation == null)
            return NotFound();

        existingLocation.City = location.City;
        existingLocation.Latitude = location.Latitude;
        existingLocation.Longitude = location.Longitude;
        existingLocation.Address = location.Address;

        _context.Entry(existingLocation).State = EntityState.Modified;

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
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LocationExists(int id)
    {
        return _context.Locations.Any(e => e.LocationId == id);
    }
    
}