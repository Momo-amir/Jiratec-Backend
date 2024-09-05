using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivityLogController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetActivityLogs()
        {
            return await _context.ActivityLog.ToListAsync(); // Correct DbSet name to plural
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityLog>> GetActivityLog(int id)
        {
            var activityLog = await _context.ActivityLog.FindAsync(id); // Use plural DbSet name

            if (activityLog == null)
            {
                return NotFound();
            }

            return activityLog;
        }

        [HttpPost]
        public async Task<ActionResult<ActivityLog>> CreateActivityLog(ActivityLog activityLog)
        {
            _context.ActivityLog.Add(activityLog); // Use plural DbSet name
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivityLog), new { id = activityLog.LogID }, activityLog); // Correctly use 'LogID'
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivityLog(int id, ActivityLog activityLog)
        {
            if (id != activityLog.LogID) // Correctly use 'LogID'
            {
                return BadRequest();
            }

            _context.Entry(activityLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ActivityLog.Any(e => e.LogID == id)) // Correctly use 'LogID'
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
            var activityLog = await _context.ActivityLog.FindAsync(id); // Use plural DbSet name
            if (activityLog == null)
            {
                return NotFound();
            }

            _context.ActivityLog.Remove(activityLog); // Use plural DbSet name
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
