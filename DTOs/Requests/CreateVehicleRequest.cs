using System.ComponentModel.DataAnnotations;
using TaskFleet.Enums;

namespace TaskFleet.DTOs.Requests;

public class CreateVehicleRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public VehicleType Type { get; set; }

    [Required]
    public int Capacity { get; set; }
}