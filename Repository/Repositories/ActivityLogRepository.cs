using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly AppDbContext _context;

        public ActivityLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivityLog?>> GetAllActivityLogsAsync()
        {
            return await _context.ActivityLog.ToListAsync();
        }

        public async Task<ActivityLog?> GetActivityLogByIdAsync(int id)
        {
            return await _context.ActivityLog.FindAsync(id);
        }

        public async Task AddActivityLogAsync(ActivityLog activityLog)
        {
            _context.ActivityLog.Add(activityLog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActivityLogAsync(ActivityLog activityLog)
        {
            _context.Entry(activityLog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteActivityLogAsync(int id)
        {
            var activityLog = await GetActivityLogByIdAsync(id);
            if (activityLog != null)
            {
                _context.ActivityLog.Remove(activityLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}