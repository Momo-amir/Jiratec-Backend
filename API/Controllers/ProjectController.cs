using Microsoft.AspNetCore.Mvc; // Import necessary namespace for creating API controllers
using DAL.Models; // Import the namespace for the Project and other models
using DAL.Data; // Import the namespace for the database context
using Microsoft.EntityFrameworkCore; 


namespace API.Controllers // Define the namespace for the controller
{
    // Define the API route and indicate that this is an API controller
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context; // Define a private variable to hold the database context

        // Constructor to initialize the database context
        public ProjectController(AppDbContext context)
        {
            _context = context; // Assign the passed context to the private variable
        }

        // GET: api/Project
        // Define an asynchronous method to get all projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            // Use Entity Framework to asynchronously get all projects from the database
            var projects = await _context.Projects.Include(p => p.CreatedByUser).ToListAsync();

            // Return an OK response with the list of projects
            return Ok(projects);
        }

        // GET: api/Project/5
        // Define an asynchronous method to get a specific project by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            // Use Entity Framework to asynchronously find a project by ID, including the related CreatedByUser
            var project = await _context.Projects.Include(p => p.CreatedByUser).FirstOrDefaultAsync(p => p.ProjectID == id);

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
            // Add the new project to the context
            _context.Projects.Add(project);
            
            // Save changes to the database
            await _context.SaveChangesAsync();

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

            // Mark the project entity as modified in the context
            _context.Entry(project).State = EntityState.Modified;

            try
            {
                // Attempt to save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If the project does not exist in the database, return a 404 Not Found response
                if (!_context.Projects.Any(e => e.ProjectID == id))
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
            // Use Entity Framework to asynchronously find a project by ID
            var project = await _context.Projects.FindAsync(id);

            // If the project is not found, return a 404 Not Found response
            if (project == null)
            {
                return NotFound();
            }

            // Remove the project from the context
            _context.Projects.Remove(project);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a 204 No Content response to indicate the deletion was successful
            return NoContent();
        }
    }
}
