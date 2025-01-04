namespace TaskFleet.DTOs.Requests;

public class UpdateTicketRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AssignedUserId { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
}
