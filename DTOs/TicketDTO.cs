using TaskFleet.Enums;

namespace TaskFleet.DTOs;

public class TicketDTO
{
    public int TicketId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    public string AssignedUserId { get; set; }
    public string AssignedUserName { get; set; }
    public string AssignedVehicleName { get; set; }
    public string StartLocationCity { get; set; }
    public string EndLocationCity { get; set; }
    public TicketStatus Status { get; set; }
}
