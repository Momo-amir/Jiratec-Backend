using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using Repository.Interfaces; // Include the repository interface
using TaskModel = DAL.Models.Task; // Alias for DAL.Models.Task
using Task = System.Threading.Tasks.Task; // Alias for System.Threading.Tasks.Task
using System.Collections.Generic;
using System.Linq; // For LINQ operations

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

        // GET: api/Task
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> GetTasks()
        {
            try
            {
                var tasks = await _taskRepository.GetAllTasksAsync(); // Fetch tasks using repository

                // Manually map TaskModel to TaskDTO
                var taskDtos = tasks.Select(MapTaskToDto).ToList();

                return Ok(taskDtos); // Return a 200 OK response with TaskDTOs
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

        // GET: api/Task/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDTO>> GetTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id); // Find the task using repository

            if (task == null)
            {
                return NotFound(); // Return 404 if task is not found
            }

            // Map TaskModel to TaskDTO
            var taskDto = MapTaskToDto(task);

            return Ok(taskDto); // Return 200 OK with the task DTO
        }

        // POST: api/Task
        [HttpPost]
        public async Task<ActionResult<TaskDTO>> CreateTask(TaskDTO taskDto)
        {
            // Manually map TaskDTO to TaskModel
            var task = MapDtoToTask(taskDto);

            await _taskRepository.AddTaskAsync(task); // Add a new task using repository

            // Return the created task DTO
            return CreatedAtAction(nameof(GetTask), new { id = task.TaskID }, MapTaskToDto(task));
        }

        // DELETE: api/Task/{id}
        [HttpDelete("{id}")] // specify an Id - only deletes the intended task ;)
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id); // Find the task using repository

            if (task == null)
            {
                return NotFound(); // Return 404 if task is not found
            }

            await _taskRepository.DeleteTaskAsync(id); // Delete task using repository
            return NoContent(); // Return 204 No Content
        }

        // Helper method to manually map a Task entity to a TaskDTO
        private TaskDTO MapTaskToDto(TaskModel task)
        {
            return new TaskDTO
            {
                TaskID = task.TaskID,
                Title = task.Title,
                Description = task.Description,
                AssignedTo = task.AssignedTo,
                AssignedToName = task.AssignedToUser?.Select(u => u.Name).FirstOrDefault(), // Get the first assigned user name
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate
            };
        }

        // Helper method to manually map a TaskDTO to a Task entity
        private TaskModel MapDtoToTask(TaskDTO taskDto)
        {
            return new TaskModel
            {
                TaskID = taskDto.TaskID,
                Title = taskDto.Title,
                Description = taskDto.Description,
                AssignedTo = taskDto.AssignedTo,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                DueDate = taskDto.DueDate
                // Additional mappings can be added here
            };
        }
    }
}
