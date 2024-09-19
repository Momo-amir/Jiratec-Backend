using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Repository.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<IEnumerable<Project>> GetProjectsForUserAsync(int userId);
        Task AddProjectAsync(Project project);
        Task UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(int id);
    }
}