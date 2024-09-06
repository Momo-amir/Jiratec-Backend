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
        private readonly AppDbContext _context; // Define a private variable to hold the database context

        // Constructor to inject the database context
        public ProjectRepository(AppDbContext context)
        {
            _context = context; // Assign the passed context to the private variable
        }

        // Retrieve all projects
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects.Include(p => p.CreatedByUser).ToListAsync();
        }

        // Retrieve a specific project by ID
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.Include(p => p.CreatedByUser).FirstOrDefaultAsync(p => p.ProjectID == id);
        }

        // Add a new project to the database
        public async Task AddProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        // Update an existing project in the database
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Delete a project from the database
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