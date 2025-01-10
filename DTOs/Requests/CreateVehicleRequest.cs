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

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Capacity must be greater than zero!")]
    public int Capacity { get; set; } = 1;
    public int? AssignedTicketId { get; set; }
}