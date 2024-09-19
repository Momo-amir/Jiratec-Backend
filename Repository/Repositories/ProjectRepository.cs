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

            public async Task<IEnumerable<Project>> GetAllProjectsAsync()
            {
                return await _context.Projects
                    .Include(p => p.CreatedBy)
                    .Include(p => p.Users)
                    .Include(p => p.Tasks)
                    .ToListAsync();
            }

            public async Task<Project?> GetProjectByIdAsync(int id)
            {
                return await _context.Projects
                    .Include(p => p.CreatedBy)
                    .Include(p => p.Users)
                    .Include(p => p.Tasks)
                    .FirstOrDefaultAsync(p => p.ProjectID == id);
            }

            public async Task<IEnumerable<Project>> GetProjectsForUserAsync(int userId)
            {
                return await _context.Projects
                    .Include(p => p.CreatedBy)
                    .Include(p => p.Users)
                    .Include(p => p.Tasks)
                    .Where(p => p.Users.Any(u => u.UserID == userId))
                    .ToListAsync();
            }

            public async Task AddProjectAsync(Project project)
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateProjectAsync(Project project)
            {
                _context.Entry(project).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

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
