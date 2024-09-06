using API.Services;
using DAL.Models;
using DAL.DTOs;
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
            // Hash the user's password before saving
            user.PasswordHash = _userService.HashPassword(user, user.PasswordHash);

            // Use the repository to add the user
            await _userRepository.AddUserAsync(user);

            // Convert User to UserDTO for response
            var userDto = new UserDTO
            {
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                RoleID = user.RoleID
            };

            // Return the user DTO
            return CreatedAtAction(nameof(GetUser), new { id = userDto.UserID }, userDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(string email, string password)
        {
            // Find the user by email using the repository
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Verify the provided password
            if (!_userService.VerifyPassword(user, user.PasswordHash, password))
            {
                return Unauthorized("Invalid credentials");
            }

            // Convert User to UserDTO for response
            var userDto = new UserDTO
            {
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                RoleID = user.RoleID
            };

            // Return user details, excluding the password hash
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
                RoleID = user.RoleID
            };

            // Return the user DTO
            return Ok(userDto);
        }
    }
}
