using Microsoft.AspNetCore.Mvc; 
using DAL.Models; 
using Repository.Interfaces; // Import the repository interfaces
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers // Define the namespace for the controller
{
    // Define the API route and indicate that this is an API controller
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository; // Define a private variable to hold the repository

        // Constructor to initialize the repository
        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository; // Assign the passed repository to the private variable
        }

        // GET: api/Project
        // Define an asynchronous method to get all projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            // Use the repository to asynchronously get all projects
            var projects = await _projectRepository.GetAllProjectsAsync();

            // Return an OK response with the list of projects
            return Ok(projects);
        }

        // GET: api/Project/5
        // Define an asynchronous method to get a specific project by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            // Use the repository to asynchronously find a project by ID
            var project = await _projectRepository.GetProjectByIdAsync(id);

            // If the project is not found, return a 404 Not Found response
            if (project == null)
            {
                return NotFound();
            }

            // If the project is found, return an OK response with the project
            return Ok(project);
        }

        // POST: api/Project
        // Define an asynchronous method to create a new project
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            // Use the repository to add the new project
            await _projectRepository.AddProjectAsync(project);

            // Return a Created response with the newly created project
            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectID }, project);
        }

        // PUT: api/Project/5
        // Define an asynchronous method to update an existing project
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            // Check if the provided project ID matches the project object ID
            if (id != project.ProjectID)
            {
                return BadRequest(); // Return a 400 Bad Request response if the IDs do not match
            }

            try
            {
                // Use the repository to update the project
                await _projectRepository.UpdateProjectAsync(project);
            }
            catch (DbUpdateConcurrencyException)
            {
                // If the project does not exist in the database, return a 404 Not Found response
                if (await _projectRepository.GetProjectByIdAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw; // If a different error occurs, rethrow the exception
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
    }
}

