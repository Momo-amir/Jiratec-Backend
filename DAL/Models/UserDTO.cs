using DAL.Enums;
using System.Collections.Generic;

namespace DAL.Models
{
    // a DTO is a Data Transfer Object, used to transfer data between layers of the application (e.g. from the API to the Repository) This DTO should probably be in the API project, not the DAL project
    public class UserDTO
    {
        public int UserID { get; set; } // Required

        public string? Name { get; set; } // Optional
        public string? Email { get; set; } // Optional
        public RoleEnum? Role { get; set; } // Optional
        public string? Token { get; set; } // Optional, used during authentication

        public List<ProjectDTO>? Projects { get; set; } // Optional, used when needed
    }
}