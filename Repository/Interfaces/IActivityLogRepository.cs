using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using DAL.Models;

namespace Repository.Interfaces
{
    public interface IActivityLogRepository
    {
        Task<IEnumerable<ActivityLog?>> GetAllActivityLogsAsync();
        Task<ActivityLog?> GetActivityLogByIdAsync(int id);
        Task AddActivityLogAsync(ActivityLog activityLog);
        Task UpdateActivityLogAsync(ActivityLog activityLog);
        Task DeleteActivityLogAsync(int id);
    }
}