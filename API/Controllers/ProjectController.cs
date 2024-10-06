using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectController(IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
            var projects = await _projectRepository.GetAllProjectsAsync();
            var projectDtos = projects.Select(p => new ProjectDTO().MapProjectToDto(p)).ToList();
            return Ok(projectDtos);
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetProject(int id)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            var projectDto = new ProjectDTO().MapProjectToDto(project);
            return Ok(projectDto);
        }

        // POST: api/Project
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProjectDTO>> CreateProject([FromBody] ProjectDTO projectDto)
        {
            try
            {
                if (projectDto == null)
                {
                    return BadRequest("Project data is required.");
                }

                if (string.IsNullOrEmpty(projectDto.Title) || string.IsNullOrEmpty(projectDto.Description))
                {
                    return BadRequest("Title and Description are required.");
                }

                // Get the current user's ID from the authentication token
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Fetch the user from the database
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                // Create a new Project entity
                var project = new Project
                {
                    Title = projectDto.Title,
                    Description = projectDto.Description,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user,
                    Users = new List<User> { user } // Assign the creator as the initial member
                };

                // Add the project to the database
                await _projectRepository.AddProjectAsync(project);

                // Map back to DTO
                var createdProjectDto = new ProjectDTO().MapProjectToDto(project);

                return CreatedAtAction(nameof(GetProject), new { id = createdProjectDto.ProjectID }, createdProjectDto);
            }
            catch (DbUpdateException dbEx)
            {
                // Log the exception and return a custom error response
                // Example: _logger.LogError(dbEx, "Database update failed");
                return StatusCode(500, "An error occurred while saving the project.");
            }
            catch (Exception ex)
            {
                // Log the exception and return a general error response
                // Example: _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        // PUT: api/Project/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDTO projectDto)
        {
            if (id != projectDto.ProjectID)
            {
                return BadRequest("Project ID mismatch.");
            }

            var existingProject = await _projectRepository.GetProjectByIdAsync(id);
            if (existingProject == null)
            {
                return NotFound("Project not found.");
            }

            // Update project properties
            existingProject.Title = projectDto.Title;
            existingProject.Description = projectDto.Description;

            // Save changes
            await _projectRepository.UpdateProjectAsync(existingProject);

            return NoContent();
        }

            // DELETE: api/Project/5
            [HttpDelete("{id}")]
            [Authorize]
            public async Task<IActionResult> DeleteProject(int id)
            {
                var project = await _projectRepository.GetProjectByIdAsync(id);
                if (project == null)
                {
                    return NotFound("Project not found.");
                }

                await _projectRepository.DeleteProjectAsync(id);

                return NoContent();
            }

        // GET: api/Project/User/Projects
        [HttpGet("User/Projects")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetCurrentUserProjects()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var projects = await _projectRepository.GetProjectsForUserAsync(userId);
            if (projects == null || !projects.Any())
            {
                return NotFound("No projects found for the current user.");
            }

            var projectDtos = projects.Select(p => new ProjectDTO().MapProjectToDto(p)).ToList();
            return Ok(projectDtos);
        }

        // POST: api/Project/{projectId}/Users
        [HttpPost("{projectId}/Users")]
        [Authorize]
        public async Task<IActionResult> AddUsersToProject(int projectId, [FromBody] List<UserDTO> userDtos)
        {
            var project = await _projectRepository.GetProjectByIdAsync(projectId);
            if (project == null)
            {
                return NotFound("Project not found.");
            }

            var userIds = userDtos.Select(u => u.UserID).ToList();
            var usersToAdd = await _userRepository.GetUsersByIdsAsync(userIds);

            foreach (var user in usersToAdd)
            {
                if (!project.Users.Any(u => u.UserID == user.UserID))
                {
                    project.Users.Add(user);
                }
            }

            await _projectRepository.UpdateProjectAsync(project);

            return NoContent();
        }
        
    }
}
