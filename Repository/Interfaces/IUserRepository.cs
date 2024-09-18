using System.Collections.Generic;
using DAL.Models; // Import User and other models from DAL.Models namespace

// Alias the System.Threading.Tasks.Task to avoid ambiguity
using Task = System.Threading.Tasks.Task;

namespace Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);

        // New method to get multiple users by their IDs
        Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<int> userIds);
        
        Task<User?> GetUserWithProjectsAsync(int userId);

        
        
    }
}
