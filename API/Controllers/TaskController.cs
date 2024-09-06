using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using Repository.Interfaces; // Include the repository interface
using TaskModel = DAL.Models.Task; // Alias for DAL.Models.Task
using Task = System.Threading.Tasks.Task; // Alias for System.Threading.Tasks.Task

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository) // Constructor with dependency injection to get access to the repository
        {
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTask()
        {
            try
            {
                var tasks = await _taskRepository.GetAllTasksAsync(); // Fetch tasks using repository
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

        [HttpDelete("{id}")] // specify an Id - only deletes the intended task ;)
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id); // Find the task using repository
            if (task == null)
            {
                return NotFound();
            }

            await _taskRepository.DeleteTaskAsync(id); // Delete task using repository
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TaskModel>> CreateTask(TaskModel task)
        {
            await _taskRepository.AddTaskAsync(task); // Add a new task using repository

            return CreatedAtAction(nameof(GetTask), new { id = task.TaskID }, task);
        }
    }
}
