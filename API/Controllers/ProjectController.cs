using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using Repository.Interfaces; // Import the repository interfaces
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Controllers // Define the namespace for the controller
{
    // Define the API route and indicate that this is an API controller
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository; // Define a private variable to hold the project repository
        private readonly IUserRepository _userRepository; // Define a private variable to hold the user repository

        // Constructor to initialize the repositories
        public ProjectController(IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository; // Assign the passed project repository to the private variable
            _userRepository = userRepository; // Assign the passed user repository to the private variable
        }

        // GET: api/Project
        // Define an asynchronous method to get all projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
            // Use the repository to asynchronously get all projects
            var projects = await _projectRepository.GetAllProjectsAsync();

            // Manually map the list of Project entities to a list of ProjectDTOs
            var projectDtos = projects.Select(p => new ProjectDTO().MapProjectToDto(p)).ToList();

            // Return an OK response with the list of project DTOs
            return Ok(projectDtos);
        }
        
        

        // GET: api/Project/5
        // Define an asynchronous method to get a specific project by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetProject(int id)
        {
            // Use the repository to asynchronously find a project by ID
            var project = await _projectRepository.GetProjectByIdAsync(id);

            // If the project is not found, return a 404 Not Found response
            if (project == null)
            {
                return NotFound();
            }

            // Manually map the Project entity to a ProjectDTO
            var projectDto = new ProjectDTO().MapProjectToDto(project);

            // If the project is found, return an OK response with the project DTO
            return Ok(projectDto);
        }

        // POST: api/Project
        // Define an asynchronous method to create a new project
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> CreateProject(ProjectDTO projectDto)
        {
            // Check if incoming DTO is null
            if (projectDto == null)
            {
                return BadRequest("Project data is required.");
            }

            // Check if the user specified in CreatedBy exists
            var user = await _userRepository.GetUserByIdAsync(projectDto.CreatedBy.UserID);
            if (user == null)
            {
                return BadRequest("The specified user does not exist.");
            }

            // Manually map the ProjectDTO to a Project entity
            var project = new Project().MapDtoToProject(projectDto);

            // Set the CreatedBy to the existing user entity
            project.CreatedBy = user;

            // Map the Users assigned to the project
            if (projectDto.Users != null && projectDto.Users.Any())
            {
                var userIds = projectDto.Users.Select(u => u.UserID).ToList();
                var users = await _userRepository.GetUsersByIdsAsync(userIds);
                project.Users = users.ToList();
            }

            // Use the repository to add the new project
            await _projectRepository.AddProjectAsync(project);

            // Manually map the created Project entity back to a ProjectDTO
            var createdProjectDto = new ProjectDTO().MapProjectToDto(project);

            // Return a Created response with the newly created project DTO
            return CreatedAtAction(nameof(GetProject), new { id = createdProjectDto.ProjectID }, createdProjectDto);
        }

        // PUT: api/Project/5
        // Define an asynchronous method to update an existing project
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectDTO projectDto)
        {
            // Check if the provided project ID matches the project object ID
            if (id != projectDto.ProjectID)
            {
                return BadRequest(); // Return a 400 Bad Request response if the IDs do not match
            }

            // Use the repository to find the existing project
            var existingProject = await _projectRepository.GetProjectByIdAsync(id);
            if (existingProject == null)
            {
                return NotFound();
            }

            // Update the project properties
            existingProject.Title = projectDto.Title;
            existingProject.Description = projectDto.Description;
            existingProject.CreatedDate = projectDto.CreatedDate;

            // Update the CreatedBy property
            var user = await _userRepository.GetUserByIdAsync(projectDto.CreatedBy.UserID);
            if (user == null)
            {
                return BadRequest("The specified user does not exist.");
            }
            existingProject.CreatedBy = user;

            // Update the Users assigned to the project
            if (projectDto.Users != null && projectDto.Users.Any())
            {
                var userIds = projectDto.Users.Select(u => u.UserID).ToList();
                var users = await _userRepository.GetUsersByIdsAsync(userIds);
                existingProject.Users = users.ToList();
            }

            try
            {
                // Use the repository to update the project
                await _projectRepository.UpdateProjectAsync(existingProject);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _projectRepository.GetProjectByIdAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Return a 204 No Content response to indicate the update was successful
            return NoContent();
        }

        // DELETE: api/Project/5
        // Define an asynchronous method to delete a project
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            // Use the repository to asynchronously find a project by ID
            var project = await _projectRepository.GetProjectByIdAsync(id);

            // If the project is not found, return a 404 Not Found response
            if (project == null)
            {
                return NotFound();
            }

            // Use the repository to delete the project
            await _projectRepository.DeleteProjectAsync(id);

            // Return a 204 No Content response to indicate the deletion was successful
            return NoContent();
        }
        [HttpGet("Project/{userId}/projects")] // Add route parameter to capture userId
        public async Task<ActionResult<object>> GetProjectsAndUsers(int userId)
        {
            // Validate the user ID
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            // Use the repository to get projects associated with the user
            var projects = await _projectRepository.GetProjectsForUserAsync(userId);

            // Check if projects are found
            if (projects == null || !projects.Any())
            {
                return NotFound("No projects found for the specified user.");
            }

            // Map the list of Project entities to a list of ProjectDTOs
            var projectDtos = projects.Select(p => new ProjectDTO().MapProjectToDto(p)).ToList();

            // Extract all users associated with these projects
            var userIds = projects.SelectMany(p => p.Users.Select(u => u.UserID)).Distinct();
            var users = await _userRepository.GetUsersByIdsAsync(userIds);

            // Create a response object containing both projects and users
            var response = new
            {
                Projects = projectDtos,
                Users = users.Select(u => new UserDTO
                {
                    UserID = u.UserID,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                }).ToList()
            };

            // Return an OK response with the list of project DTOs and users
            return Ok(response);
        }

        
    }
}
