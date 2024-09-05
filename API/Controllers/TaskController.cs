using Microsoft.AspNetCore.Mvc;       
using DAL.Data;           
using DAL.Models;         
using Microsoft.EntityFrameworkCore;
using TaskModel = DAL.Models.Task;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]

public class TaskController: ControllerBase
{
    private readonly AppDbContext _context;

    public TaskController(AppDbContext context) //Constructor with dependency injection to get access to the DBSet through AppDbContext  
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetTask()
    {
        try
        {
            var tasks = await _context.Tasks.ToListAsync();
            return Ok(tasks); // Ok is a helper method that creates an ObjectResult with a status code of 200
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest($"Error bad request - {ex.Message}");
        }
        catch (OperationCanceledException ex)
        {
            return StatusCode(499, $"Request canceled - {ex.Message}");
        }
        
    }

    [HttpDelete("{id}")] // specify an Id so you only delete the intended task ;) 
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<TaskModel>> CreateTask(TaskModel task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.TaskID }, task);
    }
}