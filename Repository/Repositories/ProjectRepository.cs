using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Repository.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all projects with related entities.
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Users)
                .Include(p => p.Tasks)
                .ToListAsync();
        }

        // Retrieves a project by its ID with related entities.
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Users)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectID == id);
        }

        // Retrieves projects associated with a specific user.
        public async Task<IEnumerable<Project>> GetProjectsForUserAsync(int userId)
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Users)
                .Include(p => p.Tasks)
                .Where(p => p.Users.Any(u => u.UserID == userId))
                .ToListAsync();
        }

        // Adds a new project to the database.
        public async Task AddProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        // Updates an existing project in the database.
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Deletes a project by its ID.
        public async Task DeleteProjectAsync(int id)
        {
            var project = await GetProjectByIdAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }
    }
}