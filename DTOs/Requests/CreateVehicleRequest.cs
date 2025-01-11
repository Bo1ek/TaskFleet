using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using TaskFleet.Enums;

namespace TaskFleet.DTOs.Requests;

public class CreateVehicleRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public VehicleType Type { get; set; }

    public int? Capacity { get; set; }
    public int? Seats { get; set; }
    public int? AssignedTicketId { get; set; }
}