namespace DAL.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; } // User ID for identification
        public string Name { get; set; } // Display name of the user
        public string Email { get; set; } // Email of the user
        public int RoleID { get; set; } // Role ID for authorization
    }
}
