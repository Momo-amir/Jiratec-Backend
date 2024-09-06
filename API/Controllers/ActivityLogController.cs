using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using Repository.Interfaces; // Include the repository interface
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogController(IActivityLogRepository activityLogRepository) // Constructor with dependency injection to get access to the repository
        {
            _activityLogRepository = activityLogRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetActivityLogs()
        {
            var activityLogs = await _activityLogRepository.GetAllActivityLogsAsync();
            return Ok(activityLogs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityLog>> GetActivityLog(int id)
        {
            var activityLog = await _activityLogRepository.GetActivityLogByIdAsync(id);

            if (activityLog == null)
            {
                return NotFound();
            }

            return activityLog;
        }

        [HttpPost]
        public async Task<ActionResult<ActivityLog>> CreateActivityLog(ActivityLog activityLog)
        {
            await _activityLogRepository.AddActivityLogAsync(activityLog);
            return CreatedAtAction(nameof(GetActivityLog), new { id = activityLog.LogID }, activityLog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivityLog(int id, ActivityLog activityLog)
        {
            if (id != activityLog.LogID)
            {
                return BadRequest();
            }

            try
            {
                await _activityLogRepository.UpdateActivityLogAsync(activityLog);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _activityLogRepository.GetActivityLogByIdAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivityLog(int id)
        {
            var activityLog = await _activityLogRepository.GetActivityLogByIdAsync(id);
            if (activityLog == null)
            {
                return NotFound();
            }

            await _activityLogRepository.DeleteActivityLogAsync(id);
            return NoContent();
        }
    }
}
