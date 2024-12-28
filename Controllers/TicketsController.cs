﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Data;
using TaskFleet.DTOs.Requests;
using TaskFleet.Models;
using TaskFleet.Models.Mappers;

namespace TaskFleet.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
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
            .ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicketById(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.AssignedUser)
            .FirstOrDefaultAsync(t => t.TicketId == id);
        if (ticket == null)
            return NotFound();
        return ticket;
    }
    
    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket(CreateTicketRequest createTicketRequest)
    {
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
}