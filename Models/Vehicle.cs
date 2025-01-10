using System.ComponentModel.DataAnnotations;
using TaskFleet.Enums;

namespace TaskFleet.Models;

public class Vehicle
{
    [Key]
    public int VehicleId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public VehicleType Type { get; set; }

    [Required] public int Capacity { get; set; } = 1;
    public bool IsAvailable { get; set; }
    public int? AssignedTicketId { get; set; }
    public Ticket? AssignedTicket { get; set; }
}