using TaskFleet.DTOs.Requests;

namespace TaskFleet.Models.Mappers;

public static class TicketMapper
{
    public static Ticket MapToDbObject(this CreateTicketRequest request)
    {
        return new Ticket()
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            CreatedDate = DateTime.Now,
            AssignedUserId = request.AssignedUserId,
            StartLocationId = request.StartLocationId,
            EndLocationId = request.EndLocationId,
            Status = request.Status,
        };
    }
}