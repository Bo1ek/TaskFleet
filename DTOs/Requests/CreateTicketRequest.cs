using System.ComponentModel.DataAnnotations;
using TaskFleet.Enums;
using TaskFleet.Models;

namespace TaskFleet.DTOs.Requests;

public class CreateTicketRequest
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    public string? AssignedUserId { get; set; }
    public int? StartLocationId { get; set; }
    public int? EndLocationId { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.WaitingForApproval;
}