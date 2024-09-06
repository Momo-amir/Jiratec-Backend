using System.Collections.Generic;
using DAL.Models; // Import User and other models from DAL.Models namespace

// Alias the System.Threading.Tasks.Task to avoid ambiguity
using Task = System.Threading.Tasks.Task;

namespace Repository.Interfaces
{
    public interface IUserRepository
    {
        // Use the alias 'Task' for all asynchronous methods to resolve the ambiguity
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task<User?>  GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}

