using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Data;
using TaskFleet.DTOs.Requests;
using TaskFleet.Models;

namespace TaskFleet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
    {
        return await _context.Vehicles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicleById(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound();
        return vehicle;
    }

    [HttpPost]
    public async Task<ActionResult<Vehicle>> CreateVehicle(CreateVehicleRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var vehicle = new Vehicle
        {
            Name = request.Name,
            Type = request.Type,
            Capacity = request.Capacity,
            Seats = request.Seats,
            AssignedTicketId = request.AssignedTicketId
        };
        
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetVehicleById), new { id = vehicle.VehicleId}, request);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Vehicle>> UpdateVehicle(int id, UpdateVehicleRequest request)
    {
        var existingVehicle = await _context.Vehicles.FindAsync(id);
        if (existingVehicle == null) return NotFound();
        
        if (!string.IsNullOrWhiteSpace(request.Name))
            existingVehicle.Name = request.Name;
        
        if (request.IsAvailable.HasValue)
            existingVehicle.IsAvailable = request.IsAvailable.Value;

        if (request.AssignedTicketId.HasValue)
            existingVehicle.AssignedTicketId = request.AssignedTicketId;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Vehicles.Any(v => v.VehicleId == id))
                return NotFound();
            else
                throw;
        }

        if (request.AssignedTicketId.HasValue)
        {
            existingVehicle.AssignedTicketId = request.AssignedTicketId;
            existingVehicle.IsAvailable = false;
        }
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound();

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
}