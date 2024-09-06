using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject the specific database context (AppDbContext)
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Use Entity Framework Core to retrieve all users
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            // Retrieve a user by their ID
            return await _context.Users.FindAsync(id);
        }

        public async Task AddUserAsync(User user)
        {
            // Add a new user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            // Update an existing user in the database
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            // Find the user by ID and remove them
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Retrieve a user by their email address
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}