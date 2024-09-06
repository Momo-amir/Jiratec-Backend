using DAL.Models;

namespace API.Services
{
    public interface IUserService
    {
        string HashPassword(User user, string password);
        bool VerifyPassword(User user, string hashedPassword, string providedPassword);
    }
}