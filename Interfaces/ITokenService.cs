using TaskFleet.Models;

namespace TaskFleet.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}