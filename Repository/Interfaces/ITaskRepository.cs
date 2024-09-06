using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Models;
using Task = System.Threading.Tasks.Task;
using TaskModel = DAL.Models.Task; // Alias for DAL.Models.Task

namespace Repository.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskModel>> GetAllTasksAsync();
        Task<TaskModel?> GetTaskByIdAsync(int id);
        Task AddTaskAsync(TaskModel task);
        Task UpdateTaskAsync(TaskModel task);
        Task DeleteTaskAsync(int id);
    }
}