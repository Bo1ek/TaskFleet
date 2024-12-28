using System.ComponentModel.DataAnnotations;

namespace TaskFleet.Models;

public class Ticket
{
    [Key]
    public int TicketId { get; set; }
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    public bool IsCompleted { get; set; } = false;
    public string? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
}