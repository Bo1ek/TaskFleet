﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Data;
using TaskFleet.DTOs.Requests;
using TaskFleet.Models;
using TaskFleet.Models.Mappers;

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
    public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
    {
        return await _context.Tickets
            .Include(t => t.AssignedUser)
            .Include(t => t.StartLocation)
            .Include(t => t.EndLocation)
            .ToListAsync();
    }
    
    [HttpGet("MyTickets")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetMyTickets()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        var tickets = await _context.Tickets
            .Where(t => t.AssignedUserId == userId)
            .Include(t => t.AssignedUser)
            .Include(t => t.StartLocation)
            .Include(t => t.EndLocation)
            .ToListAsync();

        return Ok(tickets);
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicketById(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.AssignedUser)
            .Include(t => t.StartLocation)
            .Include(t => t.EndLocation)
            .FirstOrDefaultAsync(t => t.TicketId == id);
        if (ticket == null)
            return NotFound();
        return ticket;
    }
    
    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket(CreateTicketRequest createTicketRequest)
    {
        if (!string.IsNullOrEmpty(createTicketRequest.AssignedUserId))
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == createTicketRequest.AssignedUserId);
            if (!userExists)
            {
                return BadRequest(new { Message = "Assigned user does not exist." });
            }
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
        {
            return NotFound();
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
        {
            return NotFound();
        }

        ticket.AssignedUserId = userId;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket(int id, UpdateTicketRequest updateRequest)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(updateRequest.AssignedUserId))
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == updateRequest.AssignedUserId);
            if (!userExists)
            {
                return BadRequest(new { Message = "Assigned user does not exist." });
            }
            ticket.AssignedUserId = updateRequest.AssignedUserId;
        }

        ticket.Title = updateRequest.Title ?? ticket.Title;
        ticket.Description = updateRequest.Description ?? ticket.Description;
        ticket.IsCompleted = updateRequest.IsCompleted ?? ticket.IsCompleted;

        await _context.SaveChangesAsync();
        return NoContent();
    }

}