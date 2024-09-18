using DAL.Enums;

namespace DAL.Models
{
    public class UserDTO

    {
    public int UserID { get; set; } // User ID for identification
    public string? Name { get; set; } // Display name of the user
    public string? Email { get; set; } // Email of the user

    public RoleEnum Role { get; set; } // Role of the user

    public string? Token { get; set; } // JWT Token for authentication
    
     public List<ProjectDTO> Projects { get; set; } // Optional: Only if you need to include projects in the user response
    }
}
