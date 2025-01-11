namespace TaskFleet.DTOs.Requests;

public class UpdateVehicleRequest
{
    public string? Name { get; set; }
    public bool? IsAvailable { get; set; }
    public int? AssignedTicketId { get; set; }
}