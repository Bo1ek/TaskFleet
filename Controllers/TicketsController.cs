﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Data;
using TaskFleet.DTOs;
using TaskFleet.DTOs.Requests;
using TaskFleet.Enums;
using TaskFleet.Models;
using TaskFleet.Models.Mappers;
using HostingEnvironmentExtensions = Microsoft.Extensions.Hosting.HostingEnvironmentExtensions;

namespace TaskFleet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TicketsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAllTickets()
    {
        var tickets = await _context.Tickets
            .Include(t => t.AssignedUser)
            .Include(t => t.StartLocation)
            .Include(t => t.EndLocation)
            .Include(t => t.AssignedVehicle)
            .Select(t => new TicketDTO
            {
                TicketId = t.TicketId,
                Title = t.Title,
                Description = t.Description,
                CreatedDate = t.CreatedDate,
                DueDate = t.DueDate,
                AssignedUserId = t.AssignedUserId,
                AssignedUserName = t.AssignedUser != null ? $"{t.AssignedUser.FirstName} {t.AssignedUser.LastName}" : "Unassigned",
                AssignedVehicleName = t.AssignedVehicle != null ? t.AssignedVehicle.Name : "No Vehicle Assigned",
                StartLocationCity = t.StartLocation.City,
                EndLocationCity = t.EndLocation.City,
                Status = t.Status
            })
            .ToListAsync();

        return Ok(tickets);
    }


    [HttpGet("MyTickets")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TicketDTO>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<TicketDTO>>> GetMyTickets()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { Message = "User is not authenticated." });

        var tickets = await _context.Tickets
            .Where(t => t.AssignedUserId == userId)
            .Include(t => t.AssignedUser)
            .Include(t => t.StartLocation)
            .Include(t => t.EndLocation)
            .Include(t => t.AssignedVehicle)
            .Select(t => new TicketDTO
            {
                TicketId = t.TicketId,
                Title = t.Title,
                Description = t.Description,
                CreatedDate = t.CreatedDate,
                DueDate = t.DueDate,
                AssignedUserName = t.AssignedUser != null ? $"{t.AssignedUser.FirstName} {t.AssignedUser.LastName}" : null,
                AssignedVehicleName = t.AssignedVehicle.Name,
                StartLocationCity = t.StartLocation.City,
                EndLocationCity = t.EndLocation.City,
                Status = t.Status
            })
            .ToListAsync();

        return Ok(tickets);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDTO>> GetTicketById(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.AssignedUser)
            .Include(t => t.StartLocation)
            .Include(t => t.EndLocation)
            .Include(t => t.AssignedVehicle)
            .Select(t => new TicketDTO
            {
                TicketId = t.TicketId,
                Title = t.Title,
                Description = t.Description,
                CreatedDate = t.CreatedDate,
                DueDate = t.DueDate,
                AssignedUserId = t.AssignedUserId,
                AssignedUserName = t.AssignedUser != null ? $"{t.AssignedUser.FirstName} {t.AssignedUser.LastName}" : null,
                AssignedVehicleName = t.AssignedVehicle.Name,
                StartLocationCity = t.StartLocation.City,
                EndLocationCity = t.EndLocation.City,
                Status = t.Status
            })
            .FirstOrDefaultAsync(t => t.TicketId == id);

        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }


    [HttpPost("RequestATicket")]
    public async Task<ActionResult<Ticket>> CreateTicketAsAClient(CreateTicketRequest createTicketRequest)
    {
        if (!string.IsNullOrEmpty(createTicketRequest.AssignedUserId))
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == createTicketRequest.AssignedUserId);
            if (!userExists)
                return BadRequest(new { Message = "Assigned user does not exist." });
        }

        var ticket = createTicketRequest.MapToDbObject();
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTicketById), new { id = ticket.TicketId }, ticket);
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket(CreateTicketRequest createTicketRequest)
    {
        if (!string.IsNullOrEmpty(createTicketRequest.AssignedUserId))
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == createTicketRequest.AssignedUserId);
            if (!userExists)
                return BadRequest(new { Message = "Assigned user does not exist." });
        }

        var ticket = createTicketRequest.MapToDbObject();
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTicketById), new { id = ticket.TicketId }, ticket);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        if (!string.IsNullOrEmpty(ticket.AssignedUserId))
        {
            var user = await _context.Users.FindAsync(ticket.AssignedUserId);
            if (user != null)
                user.IsAvailable = true;
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/assign/{userId}")]
    public async Task<IActionResult> AssignUserToTicket(int id, string userId)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        var user = await _context.Users.FindAsync(userId);

        if (ticket == null || user == null)
            return NotFound(new { Message = "Ticket or User not found." });

        if (!user.IsAvailable)
            return BadRequest(new { Message = "User is already assigned to another Ticket." });


        user.IsAvailable = false;
        ticket.AssignedUserId = userId;
        await _context.SaveChangesAsync();
        
        try
        {
            var emailService = new EmailService();
            string subject = "New Ticket Assignment";
            string body = $@"
            Dear {user.FirstName} {user.LastName},
            <br/><br/>
            You have been assigned to a new ticket:
            <br/>
            <b>Ticket ID:</b> {ticket.TicketId}<br/>
            <b>Title:</b> {ticket.Title}<br/>
            <b>Description:</b> {ticket.Description}<br/>
            <b>Due Date:</b> {ticket.DueDate.ToString("yyyy-MM-dd") ?? "N/A"}<br/>
            <br/>
            Best regards,<br/>
            TaskFleet Team
        ";
            await Task.Run(() => emailService.SendEmail(user.Email, subject, body));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }

        return NoContent();
    }

    [HttpPost("{ticketId}/assignVehicle/{vehicleId}")]
    public async Task<IActionResult> AssignVehicleToTicket(int ticketId, int vehicleId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return NotFound("Ticket not found");

        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle == null) return NotFound("Vehicle not found");
        if (!vehicle.IsAvailable)
            return BadRequest("Vehicle is already assigned to another ticket.");

        if (ticket.AssignedVehicleId.HasValue)
        {
            var previousVehicle = await _context.Vehicles.FindAsync(ticket.AssignedVehicleId.Value);
            if (previousVehicle != null)
            {
                previousVehicle.AssignedTicketId = 0;
                previousVehicle.IsAvailable = true;
            }
        }

        ticket.AssignedVehicleId = vehicleId;
        vehicle.AssignedTicketId = ticketId;
        vehicle.IsAvailable = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket(int id, UpdateTicketRequest updateRequest)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        if (!string.IsNullOrEmpty(updateRequest.AssignedUserId))
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == updateRequest.AssignedUserId);
            if (!userExists)
                return BadRequest(new { Message = "Assigned user does not exist." });
            ticket.AssignedUserId = updateRequest.AssignedUserId;
        }

        ticket.Title = updateRequest.Title ?? ticket.Title;
        ticket.Description = updateRequest.Description ?? ticket.Description;
        ticket.DueDate = updateRequest.DueDate ?? ticket.DueDate;
        ticket.Status = updateRequest.Status ?? ticket.Status;

        if (updateRequest.Status.HasValue)
        {
            ticket.Status = updateRequest.Status.Value;

            if (ticket.Status == TicketStatus.Completed && !string.IsNullOrEmpty(ticket.AssignedUserId))
            {
                var user = await _context.Users.FindAsync(ticket.AssignedUserId);
                if (user != null)
                {
                    user.IsAvailable = true;
                }
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("UpdateTicketStatus/{id}")]
    public async Task<IActionResult> UpdateTicketStatus(int id, UpdateTicketRequest updateRequest)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound(new { Message = "Ticket not found." });

        if (!string.IsNullOrEmpty(updateRequest.AssignedUserId))
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == updateRequest.AssignedUserId);
            if (!userExists)
                return BadRequest(new { Message = "Assigned user does not exist." });

            ticket.AssignedUserId = updateRequest.AssignedUserId;
        }

        ticket.Title = updateRequest.Title ?? ticket.Title;
        ticket.Description = updateRequest.Description ?? ticket.Description;

        if (updateRequest.Status.HasValue)
        {
            ticket.Status = updateRequest.Status.Value;

            if (ticket.Status == TicketStatus.Completed && !string.IsNullOrEmpty(ticket.AssignedUserId))
            {
                var user = await _context.Users.FindAsync(ticket.AssignedUserId);
                if (user != null)
                {
                    user.IsAvailable = true;
                }
            }
        }
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Ticket updated successfully.", Ticket = ticket });
    }
   
}