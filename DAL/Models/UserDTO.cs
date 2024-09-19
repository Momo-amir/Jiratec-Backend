using DAL.Enums;
using System.Collections.Generic;

namespace DAL.Models
{
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