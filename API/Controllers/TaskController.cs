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
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskController(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
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

        [HttpPost]
        public async Task<ActionResult<TaskDTO>> CreateTask(TaskDTO taskDto)
        {
            // Validate the incoming DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the Project exists
            var project = await _projectRepository.GetProjectByIdAsync(taskDto.ProjectID);
            if (project == null)
            {
                return NotFound("Project not found.");
            }

            // Check if the Assigned User exists
            var user = await _userRepository.GetUserByIdAsync(taskDto.AssignedTo);
            if (user == null)
            {
                return NotFound("Assigned user not found.");
            }

            // Map TaskDTO to TaskModel
            var task = MapDtoToTask(taskDto);

            // Assign navigation properties
            task.Project = project;
            task.AssignedUser = user;

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
            if (task.AssignedUser?.Name != null)
                return new TaskDTO
                {
                    TaskID = task.TaskID,
                    Title = task.Title,
                    Description = task.Description,
                    AssignedTo = task.AssignedTo,
                    AssignedToName = task.AssignedUser?.Name,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate
                };

            throw new InvalidOperationException();
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
                DueDate = taskDto.DueDate,
                ProjectID = taskDto.ProjectID
            };
        }

        
        // PUT: api/Task/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskDTO taskDto)
        {
            if (id != taskDto.TaskID)
            {
                return BadRequest("Task ID mismatch.");
            }

            var existingTask = await _taskRepository.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound(); // Task not found
            }

            // Map the updated properties
            existingTask.Title = taskDto.Title;
            existingTask.Description = taskDto.Description;
            existingTask.AssignedTo = taskDto.AssignedTo;
            existingTask.Status = taskDto.Status;
            existingTask.Priority = taskDto.Priority;
            existingTask.DueDate = taskDto.DueDate;

            await _taskRepository.UpdateTaskAsync(existingTask);

            return NoContent();
        }

    }
    
}

