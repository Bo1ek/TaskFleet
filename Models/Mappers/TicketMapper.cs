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
            IsCompleted = request.IsCompleted,
            DueDate = request.DueDate,
            CreatedDate = DateTime.Now,
            AssignedUserId = request.AssignedUserId
        };
    }
}