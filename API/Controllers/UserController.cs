using API.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Repository.Interfaces;



namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public UserController(IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }
        [HttpPost("register")]
  
        public async Task<ActionResult<UserDTO>> Register(User user)
        {
            // Check if the user already exists
            if (await _userRepository.GetUserByEmailAsync(user.Email) != null)
            {
                return BadRequest("User already exists");
            }
            // Hash the user's password before saving
            user.PasswordHash = _userService.HashPassword(user, user.PasswordHash);

            // Use the repository to add the user
            await _userRepository.AddUserAsync(user);

            // Convert User to UserDTO for response
            var userDto = new UserDTO
            {
                UserID = user.UserID, // This will be set by the database
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            // Return the user DTO
            return CreatedAtAction(nameof(GetUser), new { id = userDto.UserID }, userDto);
        }



        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest("Invalid login request.");
            }

            var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!_userService.VerifyPassword(user, user.PasswordHash, loginRequest.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate JWT Token
            var token = _userService.GenerateJwtToken(user);


            var userDto = new UserDTO
            {
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };

            return Ok(userDto);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            // Find the user by ID using the repository
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Convert User to UserDTO for response
            var userDto = new UserDTO
            {
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            // Return the user DTO
            return Ok(userDto);
        }

        [HttpGet("User/{userId}/projects")]
        public async Task<ActionResult<List<ProjectDTO>>> GetAllUsersProjectsAsync(int userId)
        {
            // Fetch the user with their projects
            var user = await _userRepository.GetUserWithProjectsAsync(userId);

            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            // Map the projects to ProjectDTOs
            var projectDtos = user.Projects.Select(p => new ProjectDTO().MapProjectToDto(p)).ToList();

            return Ok(projectDtos);
        }
        
        
        // get all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            }).ToList();
            return Ok(userDtos);
        }

    }
}
