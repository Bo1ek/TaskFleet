namespace TaskFleet.DTOs.Requests;

public class CreateLocationRequest
{
    public string City { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}